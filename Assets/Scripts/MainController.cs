using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [SerializeField]
    public Button playButton;
    [SerializeField]
    public Button pauseButton;
    [SerializeField]
    public Button resetButton;
    [SerializeField]
    public Button rescaleButton;
    [SerializeField]
    public Button randomSpriteButton;
    [SerializeField]
    public Button jumpButton;
    [SerializeField]
    public Button spinButton;
    [SerializeField]
    public Button fadeButton;
    [SerializeField]
    public Button animatedButton;
    [SerializeField]
    public GameObject animationControlUI;
    public Vector3 targetScale;
    public Sprite[] sprites;
    private Button currentActionControlButton;

    private bool pauseAnimationFlag = false;
    private bool resetButtonFlag = true;

    private Quaternion originalRotation;
    private Vector3 originalScale;

    private Vector3 originalLocation;
    private Vector3 highestJumpLocation;
    private Sprite originalSprite;
    private Color originalButtonImageColor;
    //private float animationRuntime = 4f;
    private List<UnityAction> buttonActionListeners = new List<UnityAction>();

    void OnEnable()
    {
        originalRotation = animatedButton.transform.rotation;
        originalScale = animatedButton.transform.localScale;
        originalSprite = animatedButton.GetComponent<Image>().sprite;
        originalButtonImageColor = animatedButton.GetComponent<Image>().color;
        originalLocation = animatedButton.transform.position;
    }
    void Start()
    {
        AddListenerToButton(playButton);
        AddListenerToButton(pauseButton);
        AddListenerToButton(resetButton);
        AddListenerToButton(rescaleButton);
        AddListenerToButton(randomSpriteButton);
        AddListenerToButton(jumpButton);
        AddListenerToButton(spinButton);
        AddListenerToButton(fadeButton);
        AddListenerToButton(animatedButton);
    }

    // Update is called once per frame
    void Update()
    {
        resetButton.interactable = resetButtonFlag;

        if (pauseAnimationFlag)
        {
            PauseAnimationCoroutine();
        }
    }

    void OnDisable()
    {
        RemoveAllListenerOfButton(playButton);
        RemoveAllListenerOfButton(pauseButton);
        RemoveAllListenerOfButton(resetButton);
        RemoveAllListenerOfButton(rescaleButton);
        RemoveAllListenerOfButton(randomSpriteButton);
        RemoveAllListenerOfButton(jumpButton);
        RemoveAllListenerOfButton(spinButton);
        RemoveAllListenerOfButton(fadeButton);
        RemoveAllListenerOfButton(animatedButton);
    }

    private void ButtonClicked(Button button)
    {
        Debug.Log(button.name + " is Clicked!");

        if (button.gameObject.tag == "Action Control Button")
        {
            currentActionControlButton = button;
            ActiveUI(animationControlUI);
            resetButtonFlag = true;
            //ResetButtonState(animatedButton);
        }
        else if (button.gameObject.tag == "Animation Control Button")
        {
            switch (button.name)
            {
                case "Play Button":
                    resetButtonFlag = true;
                    pauseAnimationFlag = false;
                    StartCoroutine(RunAnimationCoroutine());
                    //ResetButtonState(animatedButton);
                    break;
                case "Pause/Resume Button":
                    pauseAnimationFlag = !pauseAnimationFlag;
                    break;
                case "Reset Button":
                    ResetButtonState(animatedButton);
                    resetButtonFlag = false;

                    break;
            }
        }
        else if (button.gameObject.tag == "Untagged")
        {
            DisableUI(animationControlUI);
        }
    }

    private void ActiveUI(GameObject UI)
    {
        UI.SetActive(true);
        Debug.Log(UI.name + " is active!");
    }

    private void DisableUI(GameObject UI)
    {
        UI.SetActive(false);
        Debug.Log(UI.name + " is disabled!");
    }

    private void AddListenerToButton(Button button)
    {
        UnityAction listener = () => ButtonClicked(button);
        button.onClick.AddListener(listener);
        buttonActionListeners.Add(listener);
    }

    private void RemoveAllListenerOfButton(Button button)
    {
        foreach (var listener in buttonActionListeners)
        {
            button.onClick.RemoveListener(listener);
        }
        buttonActionListeners.Clear();
    }


    IEnumerator RunAnimationCoroutine()
    {
        float timer = 0;
        float animationDuration = 2f;
        while(timer < 4){
        Debug.Log(currentActionControlButton.name + " animation is playing");
        switch (currentActionControlButton.name)
        {
            case "Scale Button":
                StartCoroutine(ScaleCoroutine(animatedButton, targetScale));
                break;
            case "Random Sprite Button":
                StartCoroutine(ChangeSpriteCoroutine(animatedButton, sprites));
                break;
            case "Jump Button":
                StartCoroutine(JumpCoroutine(animatedButton));
                break;
            case "Spin Button":
                StartCoroutine(SpinCoroutine(animatedButton));
                break;
            case "Fade Button":
                StartCoroutine(FadeCoroutine(animatedButton));
                break;
        }

        timer += 1;
        yield return new WaitForSeconds(animationDuration);
        }
    }

    IEnumerator PauseAnimationCoroutine()
    {
        yield return new WaitUntil(() => pauseAnimationFlag == false);
    }

    public void ResetButtonState(Button button)
    {
        button.transform.rotation = originalRotation;
        button.transform.localScale = originalScale;
        button.GetComponent<Image>().sprite = originalSprite;
        button.GetComponent<Image>().color = new Color(originalButtonImageColor.r, originalButtonImageColor.g, originalButtonImageColor.b);
        button.transform.position = originalLocation;
    }

    IEnumerator ScaleCoroutine(Button button, Vector3 targetScale)
    {
        Vector3 originalScale = button.transform.localScale;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                button.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        button.transform.localScale = targetScale;
        StartCoroutine(ReturnToOriginalScaleCoroutine(button, originalScale));
    }

    IEnumerator ReturnToOriginalScaleCoroutine(Button button, Vector3 originalScale)
    {
        Vector3 currentScale = button.transform.localScale;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                button.transform.localScale = Vector3.Lerp(currentScale, originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        button.transform.localScale = originalScale;
    }

    IEnumerator ChangeSpriteCoroutine(Button button, Sprite[] sprites)
    {
        int randomIndex = Random.Range(0, sprites.Length);
        Sprite newSprite = sprites[randomIndex];
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                button.GetComponent<Image>().sprite = newSprite;
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
    }

    IEnumerator JumpCoroutine(Button button)
    {
        Vector3 originalPosition = originalLocation;
        highestJumpLocation = originalPosition;
        highestJumpLocation.y = originalPosition.y + 200;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                if (elapsed < duration)
                {
                    button.transform.position = Vector3.Lerp(originalPosition, highestJumpLocation, elapsed / duration);
                }
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        animatedButton.transform.position = highestJumpLocation;

        StartCoroutine(FallBackDownCoroutine(button, originalPosition));
    }

    IEnumerator FallBackDownCoroutine(Button button, Vector3 originalPosition)
    {

        float elapsed = 0f;
        float duration = 1f;
        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                button.transform.position = Vector3.Lerp(highestJumpLocation, originalPosition, elapsed / duration);

                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        button.transform.position = originalPosition;
    }

    IEnumerator SpinCoroutine(Button button)
    {
        float spinSpeed = 10f;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                float deltaAngle = spinSpeed * elapsed;
                button.transform.Rotate(Vector3.forward * deltaAngle);

                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        button.transform.rotation = originalRotation;
    }

    IEnumerator FadeCoroutine(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color originalColor = buttonImage.color;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            if (!pauseAnimationFlag)
            {
                float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / duration);
                buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }
}
