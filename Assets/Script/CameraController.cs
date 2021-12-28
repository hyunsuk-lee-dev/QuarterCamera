using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private RawImage[] cameraRawImages;
    [SerializeField]
    private Image[] targetImageBorders;
    
    [SerializeField]
    private Button activeButton;
    [SerializeField]
    private Sprite photoSprite;
    [SerializeField]
    private Sprite returnSprite;

    [SerializeField]
    private Image flashImage; 
    [SerializeField]
    private Text timerText;
    
    private WebCamTexture _camTexture;
    private WebCamDevice _camDevice;
    // Start is called before the first frame update
    void Start()
    {
        var devices = WebCamTexture.devices;
    
        foreach (var webCamDevice in devices)
        {
            if (webCamDevice.isFrontFacing)
            {
                _camDevice= webCamDevice;
                break;
            }
        }
        
        var sizeDelta = cameraRawImages[0].GetComponent<RectTransform>().sizeDelta;
        _camTexture = new WebCamTexture(_camDevice.name,  (int)sizeDelta.y,(int)sizeDelta.x)
        {
            filterMode = FilterMode.Trilinear,
        };
        AssignWebcamTexture();
        
        _camTexture.Play();
        
        ActiveButtonToPhoto();
    }

    private void Photo()
    {
        StartCoroutine(PhotoCoroutine());
    }

    private void Return()
    {
        AssignWebcamTexture();
        
        _camTexture.Play();
        
        ActiveButtonToPhoto();
        activeButton.gameObject.SetActive(true);
    }
    
    private IEnumerator PhotoCoroutine()
    {
        activeButton.gameObject.SetActive(false);
        var imageIndex = 0;
        while (imageIndex < cameraRawImages.Length)
        {
            yield return new WaitForSeconds(1f);

            targetImageBorders[imageIndex].enabled = true;
            
            timerText.gameObject.SetActive(true);
            var timer = 3;
            while (timer >= 0)
            {
                timerText.text = timer.ToString();
                yield return new WaitForSeconds(1f);
                timer--;
            }

            targetImageBorders[imageIndex].enabled = false;
            timerText.gameObject.SetActive(false);
            
            flashImage.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            Texture2D copyTexture = new Texture2D(_camTexture.width, _camTexture.height);
            copyTexture.SetPixels(_camTexture.GetPixels());
            copyTexture.Apply();
            yield return new WaitForEndOfFrame();
            flashImage.gameObject.SetActive(false);

            cameraRawImages[imageIndex].texture = copyTexture;
            imageIndex++;
        }
     
        _camTexture.Stop();
        
        ActiveButtonToReturn();
        activeButton.gameObject.SetActive(true);
    }

    private void AssignWebcamTexture()
    {
        foreach (var cameraRawImage in cameraRawImages)
        {
            cameraRawImage.texture = _camTexture;
        }
    }

    private void ActiveButtonToReturn()
    {
        activeButton.onClick.RemoveAllListeners();
        activeButton.image.sprite = returnSprite;
        activeButton.onClick.AddListener(Return);
    }

    private void ActiveButtonToPhoto()
    {
        activeButton.onClick.RemoveAllListeners();
        activeButton.image.sprite = photoSprite;
        activeButton.onClick.AddListener(Photo);
    }
    
}