using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    public Sprite FullHeart, HalfHeart, EmptyHeart;
    [Space]
    private Image heartImage;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    public void SetHeartImage(HeartStatus status)
    {
        switch (status)
        {
            case HeartStatus.Empty:
                heartImage.sprite = EmptyHeart;
                break;
            case HeartStatus.Half:
                heartImage.sprite = HalfHeart;
                break;
            case HeartStatus.Full:
                heartImage.sprite = FullHeart;
                break;
        }
    }
}
    public enum HeartStatus
    {
        Empty = 0,
        Half = 1,
        Full = 2
    }
