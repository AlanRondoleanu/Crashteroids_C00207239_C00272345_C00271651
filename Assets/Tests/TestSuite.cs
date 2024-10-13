using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class TestSuite
{
    private Game game;

    [SetUp]
    public void Setup()
    {
        GameObject gameGameObject = 
            UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));

        game = gameGameObject.GetComponent<Game>();
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.Destroy(game.gameObject);
    }

    [UnityTest]
    public IEnumerator AsteroidsMoveDown()
    {
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        float initialYPos = asteroid.transform.position.y;
        yield return new WaitForSeconds(0.1f);
        Assert.Less(asteroid.transform.position.y, initialYPos);
    }

    [UnityTest]
    public IEnumerator GameOverOccursOnAsteroidCollision()
    {
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        asteroid.transform.position = game.GetShip().transform.position;
        yield return new WaitForSeconds(0.1f);
        Assert.True(game.isGameOver);

    }

    [Test]
    public void NewGameRestartsGame()
    {
        game.isGameOver = true;
        game.NewGame();
        Assert.False(game.isGameOver);
    }

    [UnityTest]
    public IEnumerator LaserMovesUp()
    {
        GameObject laser = game.GetShip().SpawnLaser();
        float initialYPos = laser.transform.position.y;
        yield return new WaitForSeconds(0.1f);
        Assert.Greater(laser.transform.position.y, initialYPos);
    }

    [UnityTest]
    public IEnumerator LaserDestroysAsteroid()
    {
        // 1
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        asteroid.transform.position = Vector3.zero;
        GameObject laser = game.GetShip().SpawnLaser();
        laser.transform.position = Vector3.zero;
        yield return new WaitForSeconds(0.1f);
        // 2
        UnityEngine.Assertions.Assert.IsNull(asteroid);
    }

    [UnityTest]
    public IEnumerator DestroyedAsteroidRaisesScore()
    {
        // 1
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        asteroid.transform.position = Vector3.zero;
        GameObject laser = game.GetShip().SpawnLaser();
        laser.transform.position = Vector3.zero;
        yield return new WaitForSeconds(0.1f);
        // 2
        Assert.AreEqual(game.score, 1);
    }

    [UnityTest]
    public IEnumerator PlayerMoveLeft()
    {
        Ship ship = game.GetShip();
        float initialXPos = ship.transform.position.x;
        ship.MoveLeft();
        yield return new WaitForSeconds(0.1f);
        Assert.Less(ship.transform.position.x, initialXPos);
    }

    [UnityTest]
    public IEnumerator PlayerMoveRight()
    {
        Ship ship = game.GetShip();
        float initialXPos = ship.transform.position.x;
        ship.MoveRight();
        yield return new WaitForSeconds(0.1f);
        Assert.Greater(ship.transform.position.x, initialXPos);
    }

    [UnityTest]
    public IEnumerator CheckStaysInBoundsRight()
    {
        Ship ship = game.GetShip();
        ship.transform.position = new Vector3(-40,0,0);
        ship.MoveRight();
        yield return new WaitForSeconds(0.1f);
        Assert.GreaterOrEqual(ship.transform.position.x,-40);
    }

    [UnityTest]
    public IEnumerator CheckStaysInBoundsLeft()
    {
        Ship ship = game.GetShip();
        ship.transform.position = new Vector3(40, 0, 0);
        ship.MoveLeft();
        yield return new WaitForSeconds(0.1f);
        Assert.LessOrEqual( ship.transform.position.x,40);
    }

    [Test]
    public void NewGameSetsScoreToZero()
    {
        game.score = 2;
        game.NewGame();
        Assert.AreEqual(0, game.score);
    }

    [UnityTest]
    public IEnumerator AsteroidsMovedOffTheBottomAreDestroyed()
    {
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        asteroid.transform.position = new Vector3(0, -5, 0);
        yield return new WaitForSeconds(0.5f);
        UnityEngine.Assertions.Assert.IsNull(asteroid);
    }

    [UnityTest]
    public IEnumerator PowerUpSpawns()
    {
        Ship ship = game.GetShip();
        game.PowerupSpawnChance = 1;
        game.SpawnPowerUp(new Vector2(0,0));
        yield return new WaitForSeconds(0.1f);
        Powerup powerUp = GameObject.FindAnyObjectByType<Powerup>();

        Assert.IsNotNull(powerUp);
    }

    [UnityTest]
    public IEnumerator PowerUpSpawnsByChance()
    {
        int customSeed = 12345; //With this seed, 30 percent chance should always have this order of chance.
        int[] expectedSpawnIterations = { 1, 3, 14, 24, 26, 30, 38, 39, 47, 48 };
        int expectedIteration = 0;
        int[] actualSpawnIterations = new int[10];
        UnityEngine.Random.InitState(customSeed);

        Powerup[] powerups = GameObject.FindObjectsOfType<Powerup>();

        foreach (Powerup powerup in powerups)
        {
            GameObject.Destroy(powerup);
        }
        yield return new WaitForSeconds(0.1f);

        Powerup powerUp = null;
        for (int i = 0; i < 50; i++)
        {
            game.SpawnPowerUp(new Vector2(0, 0));
            powerUp = GameObject.FindAnyObjectByType<Powerup>();
            if (powerUp != null)
            {
                Assert.AreEqual(expectedSpawnIterations[expectedIteration], i);
                expectedIteration++;
                GameObject.DestroyImmediate(powerUp.gameObject);
            }
        }
        yield return new WaitForSeconds(0.1f);

    }

    [UnityTest]
    public IEnumerator PowerUpSpeed()
    {
        Ship ship = game.GetShip();
        ship.enabled = false;
        game.PowerupSpawnChance = 1;
        game.SpawnPowerUp(new Vector2(0, 0));
        Powerup powerUp = GameObject.FindAnyObjectByType<Powerup>();
        float startYPos = powerUp.transform.position.y;

        yield return new WaitForSeconds(1.5f);

        Assert.AreNotEqual(powerUp.transform.position.y, startYPos);
    }

    [UnityTest]
    public IEnumerator PowerUpOffscreen()
    {
        Ship ship = game.GetShip();
        game.PowerupSpawnChance = 1;
        game.SpawnPowerUp(new Vector2(0,-10));
        Powerup powerUp = GameObject.FindAnyObjectByType<Powerup>();
        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(powerUp == null);
    }

    [UnityTest]
    public IEnumerator PlayerGetsBuffed()
    {
        Ship ship = game.GetShip();
        float playerSpeed = ship.speed.Speed;
        game.PowerupSpawnChance = 1;
        game.SpawnPowerUp(ship.transform.position);
        yield return new WaitForSeconds(1f);
        float newSpeed = game.GetShip().speed.Speed;
        Assert.AreNotEqual(playerSpeed, newSpeed);
    }

    [UnityTest]
    public IEnumerator PlayerBuffExpires()
    {
        game.GetSpawner().enabled = false;
        Ship ship = game.GetShip();
        float playerSpeed = ship.speed.Speed;
        game.PowerupSpawnChance = 1;
        game.SpawnPowerUp(ship.transform.position);
        yield return new WaitForSeconds(0.1f);
        Assert.AreNotEqual(playerSpeed, ship.speed.Speed);

        yield return new WaitForSeconds(4.1f);
        ship = game.GetShip();
        Assert.AreEqual(playerSpeed, ship.speed.Speed);
    }

    public IEnumerator PlayerBuffDoesentStack()
    {
        Ship ship = game.GetShip();
        game.PowerupSpawnChance = 1;
        game.SpawnPowerUp(ship.transform.position);
        yield return new WaitForSeconds(1f);
        float playerSpeed = ship.speed.Speed;

        game.SpawnPowerUp(ship.transform.position);
        game.SpawnPowerUp(ship.transform.position);
        game.SpawnPowerUp(ship.transform.position);

        float newPlayerSpeed = ship.speed.Speed;
        Assert.AreEqual(playerSpeed, newPlayerSpeed);
    }

    // Shockwave

    [UnityTest]
    public IEnumerator ShockwaveSpawns()
    {
        Ship ship = game.GetShip();
        game.SpawnShockwave(ship.transform.position);
        yield return new WaitForSeconds(0.1f);

        Shockwave shockwave = GameObject.FindAnyObjectByType<Shockwave>();

        Assert.IsNotNull(shockwave);
    }

    [UnityTest]
    public IEnumerator ShockwaveExpires()
    {
        Ship ship = game.GetShip();
        game.SpawnShockwave(ship.transform.position);
        yield return new WaitForSeconds(0.1f);
        Shockwave shockwave = GameObject.FindAnyObjectByType<Shockwave>();

        Assert.IsNotNull(shockwave);

        yield return new WaitForSeconds(2.1f);
        Assert.IsTrue(shockwave == null);
    }

    [UnityTest]
    public IEnumerator ShockwaveSizeGrows()
    {
        bool[] sizeIncreased = { false, false, false };
        float previousShockwaveSize = 0;
        Ship ship = game.GetShip();
        game.SpawnShockwave(ship.transform.position);
        yield return new WaitForSeconds(0.1f);
        Shockwave shockwave = GameObject.FindAnyObjectByType<Shockwave>();
        previousShockwaveSize = shockwave.transform.localScale.x;

        yield return new WaitForSeconds(0.1f);
        if (previousShockwaveSize < shockwave.transform.localScale.x)
        {
            sizeIncreased[0] = true;
        }
        previousShockwaveSize = shockwave.transform.localScale.x;
        yield return new WaitForSeconds(0.1f);
        if (previousShockwaveSize < shockwave.transform.localScale.x)
        {
            sizeIncreased[1] = true;
        }

        previousShockwaveSize = shockwave.transform.localScale.x;
        yield return new WaitForSeconds(0.1f);
        if (previousShockwaveSize < shockwave.transform.localScale.x)
        {
            sizeIncreased[2] = true;
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.IsTrue(sizeIncreased[i]);
        }
    }

    [UnityTest]
    public IEnumerator ShockwaveDestroysAsteroid()
    {
        Ship ship = game.GetShip();
        ship.transform.position = new Vector3(-1, -2, 0);
        Asteroid asteroid = game.GetSpawner().SpawnAsteroid().GetComponent<Asteroid>();
        asteroid.speed = 0;
        asteroid.transform.position = Vector3.zero;
        yield return new WaitForSeconds(0.1f);

        game.SpawnShockwave(ship.transform.position);
        yield return new WaitForSeconds(1f);
        UnityEngine.Assertions.Assert.IsNull(asteroid);
    }

    [UnityTest]
    public IEnumerator ShockwaveCooldown()
    {
        GameObject shockwave1;
        GameObject shockwave2;
        Ship ship = game.GetShip();

        yield return new WaitForSeconds(0.1f);
        shockwave1 = game.SpawnShockwave(ship.transform.position);
        yield return new WaitForSeconds(0.1f);
        shockwave2 = game.SpawnShockwave(ship.transform.position);

        Assert.IsNotNull(shockwave1);
        Assert.IsNull(shockwave2);
    }

    //asteroid explosion tests
    public IEnumerator ExplosionSpawns()
    {
        Ship ship = game.GetShip();
        game.SpawnAsteroid(ship.gameObject.transform.position);//should spawn on player destroying both

        ExplosionGameObj explosion = GameObject.FindAnyObjectByType<ExplosionGameObj>();//explosion game object

        yield return new WaitForSeconds(0.1f);

        Assert.IsNotNull(explosion);
    }

    public IEnumerator ExplosionDespawns()
    {
        Ship ship = game.GetShip();
        game.SpawnAsteroid(ship.gameObject.transform.position);

        ExplosionGameObj explosion = GameObject.FindAnyObjectByType<ExplosionGameObj>();

        yield return new WaitForSeconds(3f);//explosion shouldnt exist after 3 seconds

        Assert.IsNull(explosion);
    }
}