using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TestSuite
{
    private Game game;

    [SetUp]
    public void Setup()
    {
        GameObject gameGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
        game = gameGameObject.GetComponent<Game>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(game.gameObject);
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

    // Powerup

    [UnityTest]
    public IEnumerator PowerUpTest()
    {
        Ship ship = game.GetShip();
        game.SpawnPowerUp(ship.transform.position);
        yield return new WaitForSeconds(0.1f);
        Debug.Log(ship.speed);
        Assert.AreNotEqual(5, ship.speed);
    }

    [UnityTest]
    public IEnumerator PowerUpSpawns()
    {
        Ship ship = game.GetShip();
        game.SpawnPowerUp(new Vector2(0,0));
        yield return new WaitForSeconds(0.1f);
        Powerup powerUp = GameObject.FindAnyObjectByType<Powerup>();

        Assert.IsNotNull(powerUp);
    }

    [UnityTest]
    public IEnumerator PowerUpSpeed()
    {
        Ship ship = game.GetShip();
        game.SpawnPowerUp(new Vector2(0, 0));
        Powerup powerUp = GameObject.FindAnyObjectByType<Powerup>();
        Transform startingPosition = powerUp.transform;

        yield return new WaitForSeconds(0.1f);

        Assert.AreNotEqual(powerUp.transform.position, startingPosition.position);
    }

    [UnityTest]
    public IEnumerator PowerUpOffscreen()
    {
        Ship ship = game.GetShip();
        game.SpawnPowerUp(new Vector2(0,-10));
        Powerup powerUp = GameObject.FindAnyObjectByType<Powerup>();
        yield return new WaitForSeconds(0.1f);

        Assert.IsNull(powerUp);
    }

    [UnityTest]
    public IEnumerator PlayerGetsBuffed()
    {
        Ship ship = game.GetShip();
        float playerSpeed = ship.speed;
        game.SpawnPowerUp(ship.transform.position);
        yield return new WaitForSeconds(1f);

        Assert.AreNotEqual(playerSpeed, ship.speed);
    }

    [UnityTest]
    public IEnumerator PlayerBuffExpires()
    {
        Ship ship = game.GetShip();
        float playerSpeed = ship.speed;
        game.SpawnPowerUp(ship.transform.position);
        yield return new WaitForSeconds(0.1f);
        Assert.AreNotEqual(playerSpeed, ship.speed);

        yield return new WaitForSeconds(5f);
        Assert.AreEqual(playerSpeed, ship.speed);
    }

    // Shockwave

    [UnityTest]
    public IEnumerator ShockwaveSpawns()
    {
        Ship ship = game.GetShip();
        ship.CreateShockWave();
        yield return new WaitForSeconds(0.1f);

        Shockwave shockwave = GameObject.FindAnyObjectByType<Shockwave>();

        Assert.IsNotNull(shockwave);
    }

    [UnityTest]
    public IEnumerator ShockwaveExpires()
    {
        Ship ship = game.GetShip();
        ship.CreateShockWave();
        yield return new WaitForSeconds(0.1f);
        Shockwave shockwave = GameObject.FindAnyObjectByType<Shockwave>();

        Assert.IsNotNull(shockwave);

        yield return new WaitForSeconds(2.1f);
        Assert.IsNull(shockwave);
    }

    [UnityTest]
    public IEnumerator ShockwaveSizeGrows()
    {
        bool[] sizeIncreased = { false, false, false };
        float previousShockwaveSize = 0;
        Ship ship = game.GetShip();
        ship.CreateShockWave();
        yield return new WaitForSeconds(0.1f);
        Shockwave shockwave = GameObject.FindAnyObjectByType<Shockwave>();
        previousShockwaveSize = shockwave.transform.localScale.x;

        yield return new WaitForSeconds(0.5f);
        if (previousShockwaveSize < shockwave.transform.localScale.x) {
            sizeIncreased[0] = true;
        }
        previousShockwaveSize = shockwave.transform.localScale.x;
        yield return new WaitForSeconds(0.5f);
        if (previousShockwaveSize < shockwave.transform.localScale.x)
        {
            sizeIncreased[1] = true;
        }

        previousShockwaveSize = shockwave.transform.localScale.x;
        yield return new WaitForSeconds(0.4f);
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

        ship.CreateShockWave();
        yield return new WaitForSeconds(1f);
        UnityEngine.Assertions.Assert.IsNull(asteroid);
    }

    [UnityTest]
    public IEnumerator ShockwaveCooldown()
    {
        Ship ship = game.GetShip();
        for (int i = 0; i < 10; i++)
        {
            ship.CreateShockWave();
            yield return new WaitForSeconds(0.12f);
        }
        Shockwave [] shockwaves = GameObject.FindObjectsOfType<Shockwave>();
        Assert.AreEqual(1, shockwaves.Length);
    }

}