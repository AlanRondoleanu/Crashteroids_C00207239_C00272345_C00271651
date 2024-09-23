using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

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
}