using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;using UnityEngine.UI;

public class Flipbook : MonoBehaviour
{
    private int pageCounter, clickCounter, textCounter;
    public Sprite[] pages;
    public string[] texts;
    public Image introBG, currentPage, flash;
    public GameObject overlay;
    public AudioSource sfxSource, bgmSource;
    public AudioClip crack, weAre1, weAre2, pageSwipe, talkBlip, shock, kidsPlaying;
    public TMP_Text textDisplay;
    public bool fastForward, active;

    // Start is called before the first frame update
    void Start()
    {
        sfxSource = GetComponent<AudioSource>();
        currentPage = transform.Find("Page").GetComponent<Image>();
        bgmSource = currentPage.GetComponent<AudioSource>();
        flash = transform.Find("Flash").GetComponent<Image>();
        overlay = transform.Find("Overlay").gameObject;
        StartCoroutine(IntroSequence());
        pageCounter = 0;
        clickCounter = 0;
        textCounter = 0;
        active = false;
        fastForward = false;
        sfxSource.Stop();
        flash.gameObject.SetActive(false);
        CheckConditions();
        bgmSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Input.GetKeyDown("right"))
            {
                IncrementCounter();
            }

            if (Input.GetKeyDown("left"))
            {
                //DecrementCounter();
            }

            if (Input.GetKeyDown("z"))
            {
                if (overlay.activeInHierarchy)
                {
                    overlay.SetActive(false);
                }
                else
                {
                    overlay.SetActive(true);
                }
            }
        }
        
        if (Input.GetKeyDown("x"))
        {
            fastForward = !fastForward;
            var fastForwardText = transform.Find("Overlay").Find("Help Icons").Find("Fast Forward")
                .Find("Text").GetComponent<TMP_Text>();

            if (fastForward)
            {
                fastForwardText.color = Color.yellow;
            }
            else
            {
                fastForwardText.color = Color.white;  
            }
        }
    }

    private IEnumerator IntroSequence()
    {
        var intro = transform.Find("Intro");
        introBG = intro.Find("Image").GetComponent<Image>();
        intro.gameObject.SetActive(true);
        LeanTween.value(introBG.gameObject, setIntroColorCallback, Color.black, Color.white, 1.5f);

        yield return new WaitForSeconds(2.5f);
        LeanTween.value(introBG.gameObject, setIntroColorCallback, Color.white, Color.clear, 1.5f);
        yield return new WaitForSeconds(1.7f);
        intro.gameObject.SetActive(false);
        active = true;

    }

    private void setIntroColorCallback( Color c )
    {
        introBG.color = c;
 
        // For some reason it also tweens my image's alpha so to set alpha back to 1 (I have my color set from inspector). You can use the following
 
        var tempColor = introBG.color;
        tempColor.a = 1f;
        introBG.color = tempColor;
    }

    private void CheckConditions()
    {
        switch (clickCounter)
        {
            case 1:
                NextPage();
                bgmSource.clip = kidsPlaying;
                bgmSource.Play();
                break;
            case 2:
                NextPage();
                break;
            case 6:
                NextPage();
                break;
            case 8:
                NextPage();
                bgmSource.Stop();
                bgmSource.clip = weAre1;
                bgmSource.Play();
                break;
            case 9:
                NextPage();
                break;
            case 11:
                NextPage();
                sfxSource.PlayOneShot(crack);
                break;
            case 13:
                NextPage();
                break;
            case 16:
                bgmSource.Stop();
                break;
            case 19:
                NextPage();
                break;
            case 21:
                NextPage();
                break;
            case 23:
                NextPage();
                break;
            case 29:
                NextPage();
                break;
            case 36:
                NextPage();
                break;
            case 43:
                StartCoroutine(FlashEvent());
                break;
            case 48:
                NextPage();
                break;
            case 64:
                NextPage();
                sfxSource.Stop();
                break;
            case 69:
                NextPage();
                break;
        }
    }

    private void IncrementCounter()
    {
        if (clickCounter < 69)
        {
            clickCounter++;
            textDisplay.text = "";
            CheckConditions();
            StartCoroutine(DisplayText());
            Debug.Log("TextNumber: " + clickCounter.ToString());
        }

    }

    private void DecrementCounter()
    {
        clickCounter--;
        textDisplay.text = "";
        CheckConditions();
    }

    private void NextPage()
    {
        sfxSource.PlayOneShot(pageSwipe);
        textDisplay.text = "";
        currentPage.sprite = pages[pageCounter];
        pageCounter++;
    }

    private IEnumerator FlashEvent()
    {
        NextPage();
        active = false;
        overlay.SetActive(false);
        flash.gameObject.SetActive(true);
        sfxSource.PlayOneShot(shock);
        yield return new WaitForSeconds(0.1f);
        flash.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        flash.gameObject.SetActive(true);
        sfxSource.PlayOneShot(shock);
        yield return new WaitForSeconds(0.5f);
        flash.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        flash.gameObject.SetActive(true);
        sfxSource.PlayOneShot(shock);
        yield return new WaitForSeconds(0.5f);
        flash.gameObject.SetActive(false);
        NextPage();
        yield return new WaitForSeconds(0.6f);
        flash.gameObject.SetActive(true);
        sfxSource.PlayOneShot(shock);
        yield return new WaitForSeconds(0.7f);
        flash.gameObject.SetActive(false);
        sfxSource.PlayOneShot(shock);
        yield return new WaitForSeconds(1f);
        active = true;
        overlay.SetActive(true);

        bgmSource.clip = weAre2;
        bgmSource.Play();
        StartCoroutine(StartFade(0.5f));
        
        IncrementCounter();
    }
    
    private IEnumerator StartFade(float duration)
    {
        float currentTime = 0;
        float start = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(start, 1f, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    private IEnumerator DisplayText()
    {
        active = false;
        foreach (var character in texts[textCounter])
        {
            if (!fastForward)
            {
                yield return new WaitForSeconds(0.04f);
                //sfxSource.PlayOneShot(talkBlip);
                textDisplay.text += character;
            }
            else
            {
                textDisplay.text = texts[textCounter];
            }

            if (textDisplay.text == texts[textCounter])
            {
                active = true;
            }
        }

        textCounter++;
    }
}
