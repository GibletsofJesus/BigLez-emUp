using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiScroller : MonoBehaviour
{
    [Header("Menu creation")]
    [SerializeField]
    GameObject SampleMenuElement;
    public int DesiredMenuSize;
    List<Text> TextElements = new List<Text>();
    public MenuOption[] options;
    [Header("Transition preferences")]
    public float ScrollSpeed,transitionSpeed;
    public Vector3 maxItemScale = Vector3.one;
    public Vector3 minItemScale = Vector3.one / 2;
    public Vector2 elementOffset = new Vector2(0,150);
    public AnimationCurve curve;
    public Color CenterColour, EdgeColour;
    [SerializeField]
    int menuIndex;
    private int midPoint;

    // Use this for initialization
    void Start()
    {
        menuIndex = (((DesiredMenuSize - 1) / 2)) % options.Length;

        for (int i = 0; i < DesiredMenuSize; i++)
        {
            GameObject newText = Instantiate(SampleMenuElement) as GameObject;
            newText.transform.parent = transform;
            newText.transform.localRotation = Quaternion.Euler(Vector3.zero);
            newText.GetComponent<RectTransform>().anchoredPosition3D = (i * -elementOffset) + (((DesiredMenuSize - 1) * elementOffset) / 2);
            newText.transform.localScale = (i == (DesiredMenuSize - 1) / 2) ? maxItemScale : minItemScale;
            TextElements.Add(newText.GetComponent<Text>());
            TextElements[i].text = options[(menuIndex + i) % options.Length].title;
        }
        midPoint = (TextElements.Count - 1) / 2;
    }

    float moveCD;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && moveCD <= 0)
        {
            moveCD = ScrollSpeed;
            //If index less than 0, loop back to the top.
            menuIndex = (menuIndex - 1) < 0 ? TextElements.Count - 1 : menuIndex - 1;
            MenuScroll(-1);
        }
        if (Input.GetKey(KeyCode.S) && moveCD <= 0)
        {
            moveCD = ScrollSpeed;
            //If index larger than max, go to 0.
            menuIndex = (menuIndex + 1) > TextElements.Count - 1 ? 0 : menuIndex + 1;
            MenuScroll(1);
        }

        moveCD = moveCD > 0 ? moveCD - Time.deltaTime : 0;
    }
    
    void MenuScroll(int direction)
    {
        /*mid += direction;
        if (mid > TextElements.Count-1)
            mid = 0;
        if (mid < 0)
            mid = TextElements.Count - 1;*/

        #region swap bottom / top text elements for looping effect
        Text last = TextElements[TextElements.Count - 1];
        Text first = TextElements[0];
        if (direction > 0)
        {
            //Items are going to be moving UP since selection is DOWN
            //i.e. the first item in the array goes to the last position
            TextElements.Remove(TextElements[0]);
            TextElements.Add(first);
            TextElements[TextElements.Count - 1].rectTransform.anchoredPosition = last.rectTransform.anchoredPosition -elementOffset;
        }
        else
        {
            TextElements.Remove(TextElements[TextElements.Count - 1]);
            TextElements.Insert(0,last);
            TextElements[0].rectTransform.anchoredPosition = first.rectTransform.anchoredPosition + elementOffset;
        }
        #endregion

        for (int i = 0; i < TextElements.Count; i++)
        {
            StartCoroutine(moveItem(TextElements[i], elementOffset * direction, transitionSpeed));

            #region fade items
            
            if (i < midPoint)
            {
                //StartCoroutine(fadeItem(TextElements[i], Mathf.Lerp(0, 1, (float)i / midPoint), transitionSpeed));
                StartCoroutine(fadeItem(TextElements[i], Color.Lerp(EdgeColour, CenterColour, (float)i / midPoint), transitionSpeed));
            }
            else if (i > midPoint)
            {
                //StartCoroutine(fadeItem(TextElements[i], Mathf.Lerp(0, 1, 1 - ((float)(i - midPoint) / midPoint)), transitionSpeed));
                StartCoroutine(fadeItem(TextElements[i], Color.Lerp(EdgeColour, CenterColour, 1 - ((float)(i - midPoint) / midPoint)), transitionSpeed));
            }
            else
            {
                StartCoroutine(fadeItem(TextElements[i], CenterColour, transitionSpeed));
            }

            //if end fade value if 0, loop elements
            //Also consider this is inside a loop.

            #endregion
            
            #region scale items
            if (i == midPoint)
            {
                Debug.Log(TextElements[i].text);
                StartCoroutine(ScaleItem(TextElements[i], maxItemScale, transitionSpeed*1.25f,curve));
            }
            else
            {
                StartCoroutine(ScaleItem(TextElements[i], minItemScale, transitionSpeed*1.25f));
            }
            #endregion            
        }
    }

    IEnumerator fadeItem(Text _text, float endAlpha, float _time)
    {
        Color start = _text.color;
        Color end = start;
        end.a = endAlpha;
        float currentTime = 0;
        yield return new WaitForEndOfFrame();
        while (currentTime < _time)
        {
            currentTime += Time.deltaTime;
            _text.color = Color.Lerp(start, end, currentTime / _time);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator fadeItem(Text _text, Color endColour, float _time)
    {
        Color start = _text.color;
        float currentTime = 0;
        yield return new WaitForEndOfFrame();
        while (currentTime < _time)
        {
            currentTime += Time.deltaTime;
            _text.color = Color.Lerp(start, endColour, currentTime / _time);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator moveItem(Text _text, Vector2 movement, float _time)
    {
        Vector2 start = _text.rectTransform.anchoredPosition;
        float currentTime = 0;
        yield return new WaitForEndOfFrame();
        while (currentTime < _time)
        {
            currentTime += Time.deltaTime;
            _text.rectTransform.anchoredPosition = Vector2.Lerp(start, start + movement, currentTime / _time);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ScaleItem(Text Text_, Vector2 _scale, float _time)
    {
        Vector2 InitialScale = Text_.transform.localScale;
        float currentTime = 0;
        yield return new WaitForEndOfFrame();
        while (currentTime < _time)
        {
            currentTime += Time.deltaTime;
            Text_.transform.localScale = Vector2.Lerp(InitialScale, _scale, currentTime / _time);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ScaleItem(Text _text, Vector2 _scale, float _time, AnimationCurve _curve)
    {
        Vector2 InitialScale = _text.transform.localScale;
        float currentTime = 0;
        yield return new WaitForEndOfFrame();
        while (currentTime < _time)
        {
            currentTime += Time.deltaTime;
            _text.transform.localScale = LerpWithoutClamp(InitialScale, _scale, _curve.Evaluate(currentTime / _time));

            yield return new WaitForEndOfFrame();
        }
    }

    Vector3 LerpWithoutClamp(Vector3 A, Vector3 B, float t)
    {
        return A + (B - A) * t;
    }
}
