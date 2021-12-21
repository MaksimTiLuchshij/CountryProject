using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
	public static bool sceneEnd;
	private float fadeSpeed = 4f;
	public static string nextLevel;
	private GameObject canvas;
	private Image _image;
	private bool sceneStarting;


	private GameObject upLeftImage;
	private GameObject downRightImage;
	private RectTransform rectTransform;
	private RectTransform rectTransform2;
	private Vector2 contentVector;
	private Vector2 contentVector2;
	public static bool isStartLoad = false;
	private float speed;
	private float xPositionStart = 0;
	private float yPositionStart = 0;

	[Range(0f, 1f)] //1
	public float AlphaLevel = 1f; //2

	void Awake()
	{
		Application.targetFrameRate = 300;
		//Картинка с черным квадратом, который в дальнейшем будет использоваться для затемнения / осветления сцены при переходах
		_image = GetComponent<Image>();
		
		_image.enabled = true;
		//По умолчанию у нас всегда сначала начало сцены
		sceneStarting = true;
		sceneEnd = false;

		//Находим канвас по названию
		canvas = GameObject.Find("Canvas");

		//Создаем два GameObject, которые будут участвовать в переходе между сценами как картинки
		upLeftImage = new GameObject("upLeftImage");
		downRightImage = new GameObject("downRightImage");

		//Задаем объектам родителя - канвас
		upLeftImage.transform.SetParent(canvas.transform);
		downRightImage.transform.SetParent(canvas.transform);

		//Добавляем GameObject'ам компонент-картинку
		upLeftImage.AddComponent<Image>();
		downRightImage.AddComponent<Image>();

		//Чтобы лишний раз не запихивать картинки в сцену решил их подгружать таким методом из папки Resources
		Sprite upleftSprite = Resources.Load<Sprite>("LoadScene/left");
		Sprite downRightSprite = Resources.Load<Sprite>("LoadScene/right");



		upLeftImage.GetComponent<Image>().sprite = upleftSprite;
		downRightImage.GetComponent<Image>().sprite = downRightSprite;

		//Определяем компоненты rectransform для дальнейшего передвижения картинок
		rectTransform = upLeftImage.GetComponent<RectTransform>();
		rectTransform2 = downRightImage.GetComponent<RectTransform>();

		// задаем размер картинкам
		//Здесь стоит заплатка для спавна картинок в рекордах. По неизвестной причине картинки во всех сценах спавнятся на весь экран, но не в рекордс.
		//В них задаю размеры в полтора раза больше
		//if (SceneManager.GetActiveScene().name == "Records")
		//{
		//	rectTransform.sizeDelta = new Vector2(37f, 37f);
		//	rectTransform2.sizeDelta = new Vector2(37f, 37f);
		//}
		//else
		//{
			rectTransform.sizeDelta = new Vector2(25f, 25f);
			rectTransform2.sizeDelta = new Vector2(25f, 25f);
		//}
		/*при изменении почему то слетает размер картинки (localscale), он становится огромный и картинки мешают нажать кнопки на экране,
		поэтому устанавливаю localScale вручную*/
		rectTransform.localScale = new Vector2(51.875f, 51.875f);
		rectTransform2.localScale = new Vector2(51.875f, 51.875f);
		// Здесь определяем локальную позицию, т.е. относительно центра canvas. Координаты задаю в Update()
		upLeftImage.GetComponent<RectTransform>().localPosition = new Vector3(xPositionStart, yPositionStart,-200);
		downRightImage.GetComponent<RectTransform>().localPosition = new Vector3(-xPositionStart, -yPositionStart,-200);

		downRightImage.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaLevel;
		upLeftImage.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaLevel;
	}
	//В апдейте в зависимости от начала или конца сцены я задаю скорость место спавна и скорость движения картинок
	void FixedUpdate()
	{
		if (sceneStarting) {
			StartScene();
			speed = 12f;
			MoveImage(-1500,1100);
				};
		if (sceneEnd) {
			EndScene();
			speed = 30f;
			MoveImage(0, 0);
		}
	}

	void StartScene()
	{
		Application.targetFrameRate = 300;
		//Черный квадрат светлеет...
		_image.color = Color.Lerp(_image.color, Color.clear, fadeSpeed * Time.deltaTime * 4);
		//нужны два условия для прекращения скрипта - чтобы квадрат стал прозрачным и чтобы раскрывающаяся картинка ушла за камеру
		if ((_image.color.a <= 0.01f) && (contentVector.x <-1300))
		{
			_image.color = Color.clear;
			//...и перестает показываться
			_image.enabled = false;
			//после старта всегда идет конец сцены и поэтому меняем scenestarting на false
			sceneStarting = false;

		}
	}
	//аналогично старту, только конец сцены
	void EndScene()
	{
		Application.targetFrameRate = 300;
		_image.enabled = true;
		_image.color = Color.Lerp(_image.color, Color.black, fadeSpeed * Time.deltaTime);

		if ((_image.color.a >= 0.95f) && (contentVector.x > -0.1))
		{
			_image.color = Color.black;
			//В конце сцены мы должны переключиться на другую сцену. Делаем это сразу отсюда, меняя в других методах статическую переменную nextlevel, 
			//отвечающую за название сцены
			SceneManager.LoadScene(nextLevel);
		}
	}
	//Метод передвигает картинки, smoothstep задает плавное движение
	void MoveImage(float xPositionEnd, float yPositionEnd)
    {
		Application.targetFrameRate = 300;
		contentVector.x = Mathf.SmoothStep(rectTransform.anchoredPosition.x, xPositionEnd, speed * 1.1f * Time.fixedDeltaTime);
		contentVector.y = Mathf.SmoothStep(rectTransform.anchoredPosition.y, yPositionEnd, speed * 1.1f * Time.fixedDeltaTime);			rectTransform.anchoredPosition = contentVector;
		contentVector2.x = Mathf.SmoothStep(rectTransform2.anchoredPosition.x, -xPositionEnd, speed * 1.1f * Time.fixedDeltaTime);
		contentVector2.y = Mathf.SmoothStep(rectTransform2.anchoredPosition.y, -yPositionEnd, speed * 1.1f * Time.fixedDeltaTime);
		rectTransform2.anchoredPosition = contentVector2;
	}
}
