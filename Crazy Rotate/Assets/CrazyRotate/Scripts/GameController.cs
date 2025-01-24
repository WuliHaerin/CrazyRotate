using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public enum Level 
{
	MEDIUM,
	HARD,
}

public class GameController : MonoBehaviour 
{
	public static GameController instance = null;
	public GameObject[] Balls;
	public GameObject[] NextBalls;
	public Vector3 SpawnPoint;
	public GameObject gameOverText;
	public GameObject restartButton;
	public bool gameOver = false;
	public Level level;
	public GameObject AdPanel;
	public bool isCancelAd;

	private Text scoreText;
	private int score;
	private List<GameObject> ballsList;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else  if (instance != null)
		{
			Destroy(gameObject);
		}
		ballsList = new List<GameObject>();
		InitGame();
	}

	public void SetAdPanel(bool a)
    {
		AdPanel.SetActive(a);
		Time.timeScale = a ? 0 : 1;
    }

	public void CancelAd()
    {
		isCancelAd = true;
		SetAdPanel(false);
    }

	void InitGame()
	{
		isCancelAd = false;
		score = 0;
		ballsList.Clear();
		scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		scoreText.text = "0";
		restartButton = GameObject.Find("RestartButton");
		gameOverText.SetActive(false);
		restartButton.SetActive(false);
		StartCoroutine("SpawnBall");
	}


	private IEnumerator SpawnBall()
	{
		while (!gameOver)
		{
			int randomIndex = Random.Range(0, Balls.Length);
			UpdateNextBall(randomIndex);
			GameObject ball = Instantiate(Balls[randomIndex], SpawnPoint, Quaternion.identity) as GameObject;
			ballsList.Add(ball);
			yield return new WaitForSeconds(1.5f);
		}
	}

	public void UpdateNextBall(int index)
	{
		for (int i = 0; i < NextBalls.Length; i++)
		{
			if (i != index)
			{
				NextBalls[i].SetActive(false);
			}
			else 
			{
				NextBalls[i].SetActive(true);
			}
		}
	}

	public void Score()
	{
		SoundManager.GetInstance().PlayScoreSound();
		score += 1;
		scoreText.text = "" + score;
	}

	public int GetScore()
	{
		return score;
	}

    public void Revive()
    {
		AdManager.ShowVideoAd("192if3b93qo6991ed0",
			(bol) => {
				if (bol)
				{
					SetAdPanel(false);
					StopCoroutine(GameOver());

					AdManager.clickid = "";
					AdManager.getClickid();
					AdManager.apiSend("game_addiction", AdManager.clickid);
					AdManager.apiSend("lt_roi", AdManager.clickid);


				}
				else
				{
					StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
				}
			},
			(it, str) => {
				Debug.LogError("Error->" + str);
				//AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
			});

    }

    public void PreOver()
    {
        SetAdPanel(true);
        StartCoroutine(GameOver());
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.2f);
        if (isCancelAd)
        {
            SoundManager.GetInstance().PlayGameOverSound();
            foreach (GameObject ball in ballsList)
            {
                Destroy(ball);
            }
            ballsList.Clear();
            gameOver = true;
            restartButton.SetActive(true);
            gameOverText.SetActive(true);
            StopCoroutine("SpawnBall");
            if (level == Level.MEDIUM)
            {
                int bestScore = PlayerPrefs.GetInt("BestScore_Medium", 0);
                if (score > bestScore)
                {
                    bestScore = score;
                    PlayerPrefs.SetInt("BestScore_Medium", bestScore);
                }
            }
            if (level == Level.HARD)
            {
                int bestScore = PlayerPrefs.GetInt("BestScore_Hard", 0);
                if (score > bestScore)
                {
                    bestScore = score;
                    PlayerPrefs.SetInt("BestScore_Hard", bestScore);
                }
            }
			AdManager.ShowInterstitialAd("1lcaf5895d5l1293dc",
	() => {
		Debug.Log("--插屏广告完成--");

	},
	(it, str) => {
		Debug.LogError("Error->" + str);
	});
		}

    }


    public void Restart()
	{
		SoundManager.GetInstance().PlayClickSound();
		gameOver = false;
		InitGame();
	}

	public void Quit()
	{
		SoundManager.GetInstance().PlayClickSound();
		Time.timeScale = 1f;
		//Application.LoadLevel("MainMenu");
		SceneManager.LoadScene("MainMenu");
	}

	public void Pause()
	{
		SoundManager.GetInstance().PlayClickSound();
		Time.timeScale = 0f;
	}

	public void Resume()
	{
		SoundManager.GetInstance().PlayClickSound();
		Time.timeScale = 1f;
	}
}
