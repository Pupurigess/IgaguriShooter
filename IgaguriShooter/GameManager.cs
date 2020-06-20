using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Opening,
        Playing,
        Clear,
        Over
    }
    public GameState currentState = GameState.Opening;

    AudioSource audioSource;

    public AudioClip start_sound;
    public AudioClip clear_sound;
    public AudioClip start_bgm;
    public AudioClip play_bgm;

    // パネル
    private GameObject panel;
    // タイトル
    private GameObject title;
    private GameObject subtitle;
    public GameObject timer;
    public GameObject score;
    // テキスト
    private Text titletext;
    private Text subtitletext;
    private Text timerText;
    private Text scoreText;

    public float totalTime;
    float seconds;

    public int totalScore;

    // ステージ
    private int stage = 1;

    public GameObject targetPrefab;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Panelゲームオブジェクトを検索し取得する
        panel = GameObject.Find("Panel");
        // Titleゲームオブジェクトを検索し取得する
        title = GameObject.Find("Title");
        //subtitle = GameObject.Find("SubTitle");

        // テキスト
        titletext = title.GetComponent<Text>();
        //subtitletext = subtitle.GetComponent<Text>();
        timerText = timer.GetComponent<Text>();
        scoreText = score.GetComponent<Text>();

        // オープニング
        GameOpening();
    }

    // Update is called once per frame
    void Update()
    {


        // ゲーム中ではなく、Spaceキーが押されたらtrueを返す。
        if (currentState == GameState.Opening && Input.GetKeyDown(KeyCode.Space) || Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            if(currentState != GameState.Playing)
                audioSource.PlayOneShot(start_sound);

            dispatch(GameState.Playing);

            
        }

        if (currentState == GameState.Playing)
        {
            if (totalTime <= 0)
            {
                dispatch(GameState.Clear);
            }
        }
    }

    void FixedUpdate()
    {
        // ゲーム中ではなく、Spaceキーが押されたらtrueを返す。
        if (currentState == GameState.Opening && Input.GetKeyDown(KeyCode.Space) || Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            dispatch(GameState.Playing);
        }

        if (currentState == GameState.Playing)
        {
            if (totalTime <= 0)
            {
                dispatch(GameState.Clear);
            }
        }

        Check("Enemy");

        if (tagObjects.Length <= 10)
        {
            float x = Random.Range(-25.0f, 25.0f);
            float y = Random.Range(1.0f, 6.5f);
            float z = Random.Range(-25.0f, 25.0f);

            Instantiate(targetPrefab, new Vector3(x, y, z), Quaternion.identity);
        }

        totalTime -= Time.deltaTime;
        seconds = totalTime;

        if (seconds <= 0)
            seconds = 0;
       
        timerText.text = "" + seconds.ToString("f2");

        scoreText.text = totalScore.ToString();
    }


    public void dispatch(GameState state)
    {
        GameState oldState = currentState;

        currentState = state;
        switch (state)
        {
            case GameState.Opening:
                GameOpening();
                break;
            case GameState.Playing:
                GameStart();
                break;
            case GameState.Clear:
                GameClear();
                break;
            case GameState.Over:
                if (oldState == GameState.Playing)
                {
                    GameOver();
                }
                break;
        }
    }

    GameObject[] tagObjects;

    void Check(string tagname)
    {
        tagObjects = GameObject.FindGameObjectsWithTag(tagname);
        Debug.Log(tagObjects.Length); //tagObjects.Lengthはオブジェクトの数
        if (tagObjects.Length == 0)
        {
            Debug.Log(tagname + "タグがついたオブジェクトはありません");
        }
    }

    void GameOpening()
    {
        currentState = GameState.Opening;

        // 動作停止
        Time.timeScale = 0;

        // タイトル名のセット
        SetTitle("IGAGURI SHOOTER", Color.white);

    }

    void GameStart()
    {
        // パネル非活性化
        panel.SetActive(false);

        // ボールの動作開始
        Time.timeScale = 1.0f;

    }

    // ゲームクリアー処理
    void GameClear()
    {
        // タイトル名のセット
        SetTitle("SCORE:"+totalScore, Color.white);

        audioSource.Stop();
        audioSource.PlayOneShot(clear_sound);

        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(0);
        }
    }

    // ゲームオーバー処理
    void GameOver()
    {
        // タイトル名のセット
        SetTitle("Game Over", Color.red);
        // ステージ初期値
        stage = 1;

        // ハイスコアの保存
        //FindObjectOfType<Score>().Save();

        // 3秒後にオープニング処理を呼び出す
        //Invoke("GameOpening", 3f);
    }

    // オープニング処理
    void SetTitle(string message, Color color)
    {
        // タイトル名のセット
        titletext.text = message;
        titletext.color = color;
        // パネル活性化
        panel.SetActive(true);
    }
}
