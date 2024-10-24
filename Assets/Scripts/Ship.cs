﻿/*
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
using static UnityEditor.FilePathAttribute;

public class ShipSpeed
{
    public float Speed { get; set; }

    public ShipSpeed(float speed)
    {
        Speed = speed;
    }
}

public class Ship : MonoBehaviour
{
    public bool isDead = false;
    public ShipSpeed speed;
    public bool canShoot = true;

    [SerializeField] private  MeshRenderer mesh;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform shotSpawn;
    [SerializeField] private GameObject ShockwaveObject;

    private PowerupEffect powerupEffect;

    private AudioSource audioSource;
    private readonly float maxLeft = 40;
    private readonly float maxRight = -40;
    private int shockwaveCooldown = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        speed = new ShipSpeed(1);
    }

    private void Update()
    {
        if(shockwaveCooldown > 0)
        {
            shockwaveCooldown--;
        }

        if (isDead)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space) && canShoot)
        {
            ShootLaser();
        }

        updatePowerup();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKey(KeyCode.E))
        {
            CreateShockWave();
        }
    }

    private void updatePowerup()
    {
        if (powerupEffect != null)
        {
            powerupEffect.Update(Time.deltaTime);
            if (!powerupEffect.isActive())
            {
                powerupEffect = null;
            }
        }

    }

    public void ShootLaser()
    {
        StartCoroutine(nameof(Shoot));
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        GameObject laserShot = SpawnLaser();
        laserShot.transform.position = shotSpawn.position;
        audioSource.PlayOneShot(audioSource.clip);
        yield return new WaitForSeconds(0.4f);
        canShoot = true;
    }

    public GameObject SpawnLaser()
    {
        GameObject newLaser = Instantiate(laser);
        newLaser.SetActive(true);
        return newLaser;
    }

    public void MoveLeft()
    {
        transform.Translate(-Vector3.left * Time.deltaTime * speed.Speed);
        if (transform.localPosition.x > maxLeft)
        {
            transform.localPosition = new Vector3(maxLeft, 0, 0);
        }
    }

    public void MoveRight()
    {
        transform.Translate(-Vector3.right * Time.deltaTime * speed.Speed);
        if (transform.localPosition.x < maxRight)
        {
             transform.localPosition = new Vector3(maxRight, 0, 0);
        }
    }

    public void Explode()
    {
        mesh.enabled = false;
        explosion.SetActive(true);
        isDead = true;
    }

    public void RepairShip()
    {
        explosion.SetActive(false);
        mesh.enabled = true;
        isDead = false;
    }

    public void ApplySpeedPowerup()
    {
        if (powerupEffect == null)
        {
            powerupEffect = new PowerupEffect(speed);
        }
    }

    public void CreateShockWave()
    {
        if(shockwaveCooldown <=  0)
        {
            shockwaveCooldown = 300;
            Instantiate(ShockwaveObject, this.transform.position, Quaternion.identity);
        }
    }
}
