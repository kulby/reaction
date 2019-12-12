using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question {
    public int id;
    public string text;
    public Tuple<string, int, bool> option1, option2;//<OptionText, NextQuestionId, UpdateRecomendation>
    public string recomendation;

    public Question(byte id, string text, Tuple<string, int, bool> option1, Tuple<string, int, bool> option2, string recomendation) {
        this.id = id;
        this.text = text;
        this.option1 = option1;
        this.option2 = option2;
        this.recomendation = recomendation;
    }
}

public class MainScript : MonoBehaviour {
    public Text arrowsPassedText, reactionSpeedText;
    public GameObject startScreen, testScreen, restartScreen, resultScreen, questionScreen, endScreen;
    public GameObject go, reactionResult;
    public bool roundStarted, roundEnded;

    private Animation arrowAnim, goAnim, reactioResultAnim;
    private Dictionary<int, string> directions;

    private string rightDirection;
    private float gyroX, gyroY, accX, accY;
    private float startTime, elapsedTime;
    private int reactionSpeed;
    private int currentRand, lastRand;
    private int arrowsPassed;
    private int result;

    public Text currentQuestionText;
    public Button currentQuestionOption1;
    public Button currentQuestionOption2;
    public Text recomendation;

    private Question[] questions;
    private int currentQuestionId;
    private string currentRecomendation;

    void Start() {
        directions = new Dictionary<int, string>(4) {
            { 0, "up" },
            { 1, "left" },
            { 2, "down" },
            { 3, "right" }
        };
        currentQuestionId = 0;
        currentRecomendation = "Дополнительные симптомы, указывающие на какие-либо заболевания не выявлены, однако скорость Вашей реакции замедлена. Уделите больше внимания спорту!";

        questions = new Question[18] {
            new Question(1, "Сколько часов в сутки Вы спите в среднем?",
                new Tuple<string, int, bool> ("6 и более", 3, false), new Tuple<string, int, bool> ("Менее 6", 2, true),
                "Некоторые симптомы указывают на синдром хронической усталости. Рекомендуем Вам обратиться к Неврологу для уточнения диагноза. Будьте здоровы!"),
            new Question(2, "За последние трое суток Ваш сон составлял больше 6 часов в сутки?",
                new Tuple<string, int, bool> ("Да", 3, false), new Tuple<string, int, bool> ("Нет", 0, true),
                "Для активной и полноценной деятельности человека, огромное значение имеет полноценный сон. Рекомендуем Вам хорошо выспаться и пройти тест заново!"),
            new Question(3, "Употребляете ли Вы алкоголь или наркотические вещества?",
                new Tuple<string, int, bool> ("Да", 4, false), new Tuple<string, int, bool> ("Нет", 5, false),
                null),

            new Question(4, "Сколько раз в неделю Вы употребляете алкоголь или наркотические вещества?",
                new Tuple<string, int, bool> ("1-2 раза", 5, false), new Tuple<string, int, bool> ("3 и более", 0, true),
                "Такое частое употребление алкоголя/наркотиков разрушительно влияет на организм и указывает на наличие зависимости. Рекомендуем обратиться к Наркологу."),
            new Question(5, "Принимаете ли Вы седативные препараты, транквилизаторы, нейролептики или антидепрессанты?",
                new Tuple<string, int, bool> ("Да", 0, true), new Tuple<string, int, bool> ("Нет", 6, false),
                "Возможно требуется коррекция дозировки. Проконсультируйтесь с Неврологом или врачем, назначащим лечение. Пройдите тест ещё раз через неделю."),
            new Question(6, "Беспокоят ли Вас головные боли?",
                new Tuple<string, int, bool> ("Да", 7, false), new Tuple<string, int, bool> ("Нет", 8, false),
                null),

            new Question(7, "Сколько раз в неделю у Вас возникают головные боли?",
                new Tuple<string, int, bool> ("1-2 раза", 8, false), new Tuple<string, int, bool> ("3 и более", 8, true),
                "Рекомендуем Вам обратиться к Неврологу для выявления причин регулярной головной боли. Будьте здоровы!"),
            new Question(8, "У Вас бывают головокружения, мелькание мушек перед глазами?",
                new Tuple<string, int, bool> ("Да", 9, true), new Tuple<string, int, bool> ("Нет", 9, false),
                "Некоторые симптомы указывают на синдром вегетативной дистонии. Рекомендуем Вам обратиться к Неврологу для уточнения диагноза. Будьте здоровы!"),
            new Question(9, "Бывают ли у Вас нарушения памяти, сознания (нарушение ориентировки в пространстве, времени)?",
                new Tuple<string, int, bool> ("Да", 10, true), new Tuple<string, int, bool> ("Нет", 10, false),
                "Некоторые симптомы указывают на синдром расстройства сознания. Рекомендуем Вам обратиться к Психиатру для уточнения диагноза. Будьте здоровы!"),

            new Question(10, "Ваше артериальное давление повышалось выше 140 / 90 мм. рт. ст.?",
                new Tuple<string, int, bool> ("Да", 11, true), new Tuple<string, int, bool> ("Нет", 11, false),
                "Некоторые симптомы указывают на синдром артериальной гипертензии. Рекомендуем Вам обратиться к Терапевту для уточнения диагноза. Будьте здоровы!"),
            new Question(11, "Бывает ли у Вас тремор рук?",
                new Tuple<string, int, bool> ("Да", 12, false), new Tuple<string, int, bool> ("Нет", 14, false),
                null),
            new Question(12, "Тремор рук сопровождается чувством голода, потливостью, паникой?",
                new Tuple<string, int, bool> ("Да", 0, true), new Tuple<string, int, bool> ("Нет", 13, false),
                "Некоторые симптомы указывают на синдром гипогликемии. Рекомендуем Вам обратиться к Эндокринологу для уточнения диагноза. Будьте здоровы!"),

            new Question(13, "Чувствуете ли Вы скованность, замедленность движений, повышенное слюноотделение?",
                new Tuple<string, int, bool> ("Да", 0, true), new Tuple<string, int, bool> ("Нет", 14, false),
                "Некоторые симптомы указывают на синдром паркинсонизма. Рекомендуем Вам обратиться к Неврологу для уточнения диагноза. Будьте здоровы!"),
            new Question(14, "Вы чувствуете постоянную усталость?",
                new Tuple<string, int, bool> ("Да", 15, false), new Tuple<string, int, bool> ("Нет", 18, false),
                null),
            new Question(15, "Замечали ли Вы в последнее время ломкость ногтей и выпадение волос?",
                new Tuple<string, int, bool> ("Да", 16, true), new Tuple<string, int, bool> ("Нет", 16, false),
                "Некоторые симптомы указывают на синдром железодефитной анемии. Рекомендуем Вам обратиться к Терапевту для уточнения диагноза. Будьте здоровы!"),

            new Question(16, "Заметили ли вы за последнее время увеличение веса, появление зябкости, мышечные боли?",
                new Tuple<string, int, bool> ("Да", 0, true), new Tuple<string, int, bool> ("Нет", 17, false),
                "Некоторые симптомы указывают на синдром гипотиреоза. Рекомендуем Вам обратиться к Эндокринологу для уточнения диагноза. Будьте здоровы!"),
            new Question(17, "Вас беспокоит повышенная утомляемость, раздражительность, нарушения сна?",
                new Tuple<string, int, bool> ("Да", 0, true), new Tuple<string, int, bool> ("Нет", 18, false),
                "Некоторые симптомы указывают на астенический синдром. Рекомендуем Вам обратиться к Неврологу для уточнения диагноза. Будьте здоровы!"),
            new Question(18, "Бывают ли у Вас судороги?",
                new Tuple<string, int, bool> ("Да", 0, true), new Tuple<string, int, bool> ("Нет", 0, false),
                "Судорожный синдром требует внимания со стороны специалиста. Рекомендуем Вам обратиться к Неврологу. Будьте здоровы!")
        };

        arrowAnim = GetComponent<Animation> ();
		goAnim = go.GetComponent<Animation> ();
		reactioResultAnim = reactionResult.GetComponent<Animation> ();
    }

    void FixedUpdate() {
		if (roundEnded == true && arrowsPassed != 15) {
			GenerateArrow ();
			roundEnded = false;
		}

		/* ACCELEROMETR */ 

		Vector3 acceleration = Input.acceleration;
		accX = acceleration.x;
		accY = acceleration.y;
		
		/* GYROSCOPE */

		Quaternion gyroQaternion = Input.gyro.attitude;
		Vector3 eulerGyro = gyroQaternion.eulerAngles;

		gyroX = eulerGyro.x;
		gyroY = eulerGyro.y;

		if (gyroX > 180f) {
			gyroX -= 360f;
		}

		if (gyroY > 180f) {
			gyroY -= 360f;
		}

        /* REACTION */

        if (roundStarted) {
            switch (rightDirection) {
                case "right":
                    if ((gyroY > 25f && accX >= 0.5f) || Input.GetKeyDown(KeyCode.RightArrow)) {
                        GoodRound();
                    } else if (gyroX > 30f || gyroX < -10f || gyroY < -25f) {
                        GameOver();
                    }
                    break;
                case "left":
                    if ((gyroY < -25f && accX <= -0.5f) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                        GoodRound();
                    } else if (gyroX > 30f || gyroX < -10f || gyroY > 25f) {
                        GameOver();
                    }
                    break;
                case "up":
                    if ((gyroX < -10f && accY >= 0.5f) || Input.GetKeyDown(KeyCode.UpArrow)) {
                        GoodRound();
                    } else if (gyroY > 20f || gyroY < -20f || gyroX > 25f) {
                        GameOver();
                    }
                    break;
                case "down":
                    if ((gyroX > 25f && accY <= -0.5f) || Input.GetKeyDown(KeyCode.DownArrow)) {
                        GoodRound();
                    } else if (gyroY > 25f || gyroY < -25f || gyroX < -5f) {
                        GameOver();
                    }
                    break;
            }
        }
    }

    /* MENU */

    public void StartTest() {
        result = 0;
        roundStarted = false;
        roundEnded = false;
        arrowsPassed = 0;
        lastRand = -1;
        reactionSpeed = 0;
        arrowsPassedText.text = "0 / 15";
        startScreen.SetActive(false);
        restartScreen.SetActive(false);
        endScreen.SetActive(false);
        resultScreen.SetActive(false);
        goAnim.Play("Go");
        testScreen.SetActive(true);
        GenerateArrow();
    }

    private void ShowResults() {
        result /= 10;
        reactionSpeedText.text = result.ToString() + " мс";
        testScreen.SetActive(false);
        resultScreen.SetActive(true);
        reactioResultAnim.Play("ReactionResultFadeIn");
    }

    public void Questions() {
        resultScreen.SetActive(false);
        currentQuestionId = 1;
        FillNewQuestion(currentQuestionId);
        questionScreen.SetActive(true);
    }

    public void EndTest() {
        questionScreen.SetActive(false);
        recomendation.text = currentRecomendation;
        endScreen.SetActive(true);
    }

    /* REACTION */

    private void GoodRound() {
        roundStarted = false;
        elapsedTime = Time.realtimeSinceStartup - startTime;
        reactionSpeed = Mathf.RoundToInt(elapsedTime * 1000);
        arrowsPassed++;
        arrowsPassedText.text = arrowsPassed.ToString() + " / 15";
        arrowAnim.Play("FadeOut");
        if (arrowsPassed > 5 && arrowsPassed < 16) {
            result += reactionSpeed;
        }
        if (arrowsPassed == 15) {
            ShowResults();
        }
    }

    private void GenerateArrow() {
        currentRand = UnityEngine.Random.Range(0, 4);
        while (currentRand == lastRand) {
            currentRand = UnityEngine.Random.Range(0, 4);
        }
        lastRand = currentRand;
        rightDirection = directions[currentRand];
        transform.eulerAngles = new Vector3(0f, 0f, currentRand * 90f);
        arrowAnim.Play("FadeIn");
        startTime = Time.realtimeSinceStartup;
    }

    private void GameOver() {
        roundStarted = false;
        transform.localScale = new Vector3(0f, 0f, 0f);
        testScreen.SetActive(false);
        restartScreen.SetActive(true);
    }

    /* QUESTIONS */

    public Question GetQuestionById(int questionId) {
        for (int i = 0; i < questions.Length; i++) {
            if (questions[i].id == questionId) {
                return questions[i];
            }
        }
        return null;
    }

    public void FillNewQuestion(int questionId) {
        if (questionId == 0) {
            EndTest();
        } else {
            Question currentQuestion = GetQuestionById(questionId);
            currentQuestionText.text = currentQuestion.text;
            currentQuestionOption1.GetComponentInChildren<Text>().text = currentQuestion.option1.Item1;
            currentQuestionOption2.GetComponentInChildren<Text>().text = currentQuestion.option2.Item1;
        }
    }

    public void SelectOption1() {
        Question question = GetQuestionById(currentQuestionId);
        if (question.option1.Item3) {
            currentRecomendation = question.recomendation;
        }
        currentQuestionId = question.option1.Item2;
        FillNewQuestion(currentQuestionId);
    }

    public void SelectOption2() {
        Question question = GetQuestionById(currentQuestionId);
        if (question.option2.Item3) {
            currentRecomendation = question.recomendation;
        }
        currentQuestionId = question.option2.Item2;
        FillNewQuestion(currentQuestionId);
    }
}
