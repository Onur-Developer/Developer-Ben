using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject[] enemyPool;
    [Range(2, 10)] public float maxSpawnTime;
    [SerializeField] private GameObject orangeMultipliedInstance;

    private void OnEnable()
    {
        StartCoroutine(SpawnEnemy());
    }


    private void OnDisable()
    {
        StopCoroutine(SpawnEnemy());
        KillChilds();
    }

    IEnumerator SpawnEnemy()
    {
        float waitTime = Random.Range(2.5f, maxSpawnTime);
        SpawnOrangeMultip();
        yield return new WaitForSeconds(waitTime);
        if (GameManager.instance.enemySpawnerCount > 0)
            StartCoroutine(SpawnEnemy());
    }

    void KillChilds()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }
    }

    void SpawnOrangeMultip()
    {
        if (GameManager.instance.enemySpawnerCount <= 0)
            return;
        Vector2 position = new Vector2(Random.Range(-16f, 16f), Random.Range(-8f, 8f));
        GameObject orangeMultiplied = Instantiate(orangeMultipliedInstance, transform);
        orangeMultiplied.transform.position = position;
        Tween createEnemyTween = orangeMultiplied.transform.DOScale(1.5f, .5f)
            .SetLoops(5, LoopType.Yoyo)
            .OnComplete((() =>
            {
                Destroy(orangeMultiplied);
                createEnemyTween = null;
                if (GameManager.instance.enemySpawnerCount <= 0)
                    return;
                GameObject enemy = Instantiate(PickSpawnEnemy(), transform);
                enemy.transform.position = position;
                GameManager.instance.enemySpawnerCount--;
            }));
    }

    GameObject PickSpawnEnemy()
    {
        int randomValue = Random.Range(0, 101);
        while (true)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i].GetComponent<Enemy>();
                if (randomValue < enemy.spawnChance)
                    return enemies[i];
                randomValue -= enemy.spawnChance;
            }
        }
    }
}