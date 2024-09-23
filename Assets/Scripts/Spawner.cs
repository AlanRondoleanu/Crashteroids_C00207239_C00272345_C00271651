/*
 * Copyright (c) 2023 Kodeco Inc.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public ObjectPool<GameObject> asteroids;
    //public List<GameObject> asteroids = new List<GameObject>();

    [SerializeField] private GameObject asteroid1;
    [SerializeField] private GameObject asteroid2;
    [SerializeField] private GameObject asteroid3;
    [SerializeField] private GameObject asteroid4;

    private Vector3 spawnPosition;
    private AudioSource audioSource;
    private IEnumerator spawnRoutine;
    
    private void Awake()
    {
        asteroids = new ObjectPool<GameObject>(SpawnAsteroid, SetPosition,
            destroyed => PlayExplosion());
        spawnPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        spawnRoutine = Spawn();
    }

    public void BeginSpawning()
    {
        StartCoroutine(spawnRoutine);
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.4f);
            SpawnAsteroid();
        }
    }

    public GameObject SpawnAsteroid()
    {
        int random = Random.Range(1, 5);
        GameObject asteroid;
        switch (random)
        {
            case 1:
                asteroid = Instantiate(asteroid1);
                break;
            case 2:
                asteroid = Instantiate(asteroid2);
                break;
            case 3:
                asteroid = Instantiate(asteroid3);
                break;
            case 4:
                asteroid = Instantiate(asteroid4);
                break;
            default:
                asteroid = Instantiate(asteroid1);
                break;
        }

        asteroid.SetActive(true);
        SetPosition(asteroid);
        return asteroid;
    }

    private void SetPosition(GameObject asteroid)
    {
        float xPos = Random.Range(-8.0f, 8.0f);

        // Spawn asteroid just above top of screen at a random point along x-axis
        asteroid.transform.position = new Vector3(xPos, spawnPosition.y, spawnPosition.z);
    }

    private void PlayExplosion()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }

    public void ClearAsteroids()
    {
        asteroids.Dispose();
    }

    public void StopSpawning()
    {
        StopCoroutine(spawnRoutine);
    }
}
