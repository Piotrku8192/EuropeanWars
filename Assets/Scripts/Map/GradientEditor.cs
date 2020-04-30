using UnityEngine;

public class GradientEditor : MonoBehaviour
{
    public Texture2D toApply;
    public Gradient gradient;

    private void OnValidate()
    {
        Color[] colors = new Color[toApply.width];

        for (int i = 0; i < toApply.width; i++)
        {
            colors[i] = gradient.Evaluate((float)i / toApply.width);
        }

        toApply.SetPixels(colors);
        toApply.Apply();
    }
}
