using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionIndicator : MonoBehaviour
{
    [SerializeField] GameObject indicatorLine;
    [SerializeField] Transform lineStartPoint;
    [SerializeField] Transform lineEndPoint;
    [SerializeField] PlayerMovementController playerMovement;

    List<GameObject> enemiesInRange = new List<GameObject>();
    GameObject closestEnemy;
    GameObject previousClosestEnemy;
    PossessionManager targetPossessionManager;
    float minimumDistanceToPlayer;
    float distanceToPlayer;
    Vector3 playerDirection;
    Vector3 playerPosition;
    Vector3 enemyPosition;

    void Update()
    {
        minimumDistanceToPlayer = 100f;

        if(enemiesInRange.Count == 0)
        {
            indicatorLine.SetActive(false);
            return;
        }

        playerPosition = transform.position;

        foreach (GameObject enemy in enemiesInRange)
        {
            enemyPosition = enemy.transform.position;
            distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
            if (distanceToPlayer < minimumDistanceToPlayer)
            {
                minimumDistanceToPlayer = distanceToPlayer;
                previousClosestEnemy = closestEnemy;
                closestEnemy = enemy;
                UpdateLine();
                if(previousClosestEnemy != null && closestEnemy != previousClosestEnemy)
                {
                    previousClosestEnemy.GetComponent<PossessionManager>().SetClosestToPlayer(false);
                }
                closestEnemy.GetComponent<PossessionManager>().SetClosestToPlayer(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        targetPossessionManager = other.GetComponent<PossessionManager>();
        if (targetPossessionManager != null && targetPossessionManager.canGetPossessed)
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    void UpdateLine()
    {
        indicatorLine.SetActive(true);
        Vector3 localScale = indicatorLine.transform.localScale;
        float lineLength = Vector3.Distance(lineStartPoint.position, lineEndPoint.position);
		localScale.x *= minimumDistanceToPlayer/lineLength;
        indicatorLine.transform.localScale = localScale;
        if (playerMovement.IsFacingRight())
        {
            playerDirection = Vector3.right;
        }
        else
        {
            playerDirection = Vector3.left;
        }
        float angle = Vector3.SignedAngle(playerDirection, closestEnemy.transform.position - transform.position, Vector3.forward);
        indicatorLine.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
