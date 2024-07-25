using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class PomodoroTimer : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image TomatoImage;
    public AudioSource AudioSource;
    public TMP_InputField inputField;
    public Button startButton;
    public Button stopButton;
    public Button CloseButton;
    public TMP_Text timerText;

    private Coroutine countdownCoroutine;
    private float timeLeft;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    
    private Color startColor = Color.green;
    private Color endColor = Color.white;
    private Vector3 originalScale;
    private bool isMouseOver = false;
    private Vector2 offset;
    void Start()
    {
        Application.runInBackground = true;
        startButton.onClick.AddListener(StartTimer);
        stopButton.onClick.AddListener(StopTimer);
        CloseButton.onClick.AddListener(CloseWindow);
        timerText.text = "25:00";
        inputField.text = "25";
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale; 
    }
    
    private void Update()
    {
        if (isMouseOver)
        {
            // 鼠标滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float newScale = Mathf.Clamp(rectTransform.localScale.x + scroll, 0.5f, 2f);
                rectTransform.localScale = new Vector3(newScale, newScale, newScale);
            }

            // 按下滚轮恢复大小
            if (Input.GetMouseButtonDown(2)) // 鼠标中键
            {
                rectTransform.localScale = originalScale; // 恢复到原始大小
            }
        }
    }

    private void CloseWindow()
    {
        Application.Quit();
    }

    void StartTimer()
    {
        TomatoImage.color= startColor;
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

        if (float.TryParse(inputField.text, out float minutes))
        {
            timeLeft = minutes * 60; // 转换为秒
            countdownCoroutine = StartCoroutine(Countdown());
        }
    }

    void StopTimer()
    {
        TomatoImage.color= endColor;
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            timerText.text = "00:00";
        }
    }

    IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerText();
            if (Mathf.FloorToInt(timeLeft) == 10 && !AudioSource.isPlaying)
            {
                AudioSource.Play();
            }
            float t = Mathf.Clamp01(timeLeft / (inputField.text == "" ? 1 : float.Parse(inputField.text) * 60));
            TomatoImage.color = Color.Lerp(endColor, startColor, t); // 从绿色变为白色
            yield return null;
        }

        TimerEnded();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    void TimerEnded()
    {
        timerText.text = "(*^▽^*)";
        // 在这里可以添加其他事件，比如音效提醒等
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = new Vector2(rectTransform.position.x, rectTransform.position.y) - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = eventData.position + offset;

        // 确保UI元素在屏幕内
        newPosition.x = Mathf.Clamp(newPosition.x, rectTransform.rect.width / 2, Screen.width - rectTransform.rect.width / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, rectTransform.rect.height / 2, Screen.height - rectTransform.rect.height / 2);

        rectTransform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 可以在这里添加拖动结束后的逻辑
    }

    private float CanvasScaleFactor()
    {
        return GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}