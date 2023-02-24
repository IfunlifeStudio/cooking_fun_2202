using UnityEngine;
using System.Collections;
using TMPro;
public class VertexRevealAnimation : MonoBehaviour
{
    public delegate void CompleteAnimationFadeIn();
    public event CompleteAnimationFadeIn OnCompleteAnimationFadeIn;
    private TMP_Text m_TextComponent;

    public float FadeSpeed = 1.0F;
    public int RolloverCharacterSpread = 10;
    public Color ColorTint, ColorTintFadeIn;


    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }
    public void StartAnimateVertexFadeInColors()
    {
        StartCoroutine(AnimateVertexFadeInColors());
    }
    public void StopAnimateVertexFadeInColors()
    {
        StopAllCoroutines();
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            Color32[] newVertexColors;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            if (vertexIndex + 0 < newVertexColors.Length)
                newVertexColors[vertexIndex + 0].a = 255;
            if (vertexIndex + 1 < newVertexColors.Length)
                newVertexColors[vertexIndex + 1].a = 255;
            if (vertexIndex + 2 < newVertexColors.Length)
                newVertexColors[vertexIndex + 2].a = 255;
            if (vertexIndex + 3 < newVertexColors.Length)
                newVertexColors[vertexIndex + 3].a = 255;
        }


        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        if (OnCompleteAnimationFadeIn != null)
            OnCompleteAnimationFadeIn();
    }


    public IEnumerator AnimateVertexFadeInColors()
    {
        // Need to force the text object to be generated so we have valid data to work with right from the start.
        m_TextComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        Color32[] newVertexColors;

        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool isRangeMax = false;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            newVertexColors[vertexIndex + 0].a = 0;
            newVertexColors[vertexIndex + 1].a = 0;
            newVertexColors[vertexIndex + 2].a = 0;
            newVertexColors[vertexIndex + 3].a = 0;
        }
        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        while (!isRangeMax)
        {
            int characterCount = textInfo.characterCount;

            // Spread should not exceed the number of characters.
            byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);


            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                // Skip characters that are not visible
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the current character's alpha value.
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);
                // Set new alpha values.
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;

                // Tint vertex colors
                // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                newVertexColors[vertexIndex + 0] = (Color)newVertexColors[vertexIndex + 0] * ColorTintFadeIn;
                newVertexColors[vertexIndex + 1] = (Color)newVertexColors[vertexIndex + 1] * ColorTintFadeIn;
                newVertexColors[vertexIndex + 2] = (Color)newVertexColors[vertexIndex + 2] * ColorTintFadeIn;
                newVertexColors[vertexIndex + 3] = (Color)newVertexColors[vertexIndex + 3] * ColorTintFadeIn;

                if (alpha == 255)
                {
                    startingCharacterRange += 1;

                    if (startingCharacterRange == characterCount)
                    {
                        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                        m_TextComponent.ForceMeshUpdate();
                        if (OnCompleteAnimationFadeIn != null)
                            OnCompleteAnimationFadeIn();
                        isRangeMax = true;
                    }
                }
            }

            // Upload the changed vertex colors to the Mesh.
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (currentCharacter + 1 < characterCount) currentCharacter += 1;

            yield return new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
        }
    }
    IEnumerator AnimateVertexFadeOutColors()
    {
        // Need to force the text object to be generated so we have valid data to work with right from the start.
        m_TextComponent.ForceMeshUpdate();


        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        Color32[] newVertexColors;

        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool isRangeMax = false;

        while (!isRangeMax)
        {
            int characterCount = textInfo.characterCount;

            // Spread should not exceed the number of characters.
            byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);


            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                // Skip characters that are not visible
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the current character's alpha value.
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a - fadeSteps, 0, 255);

                // Set new alpha values.
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;

                // Tint vertex colors
                // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                newVertexColors[vertexIndex + 0] = (Color)newVertexColors[vertexIndex + 0] * ColorTint;
                newVertexColors[vertexIndex + 1] = (Color)newVertexColors[vertexIndex + 1] * ColorTint;
                newVertexColors[vertexIndex + 2] = (Color)newVertexColors[vertexIndex + 2] * ColorTint;
                newVertexColors[vertexIndex + 3] = (Color)newVertexColors[vertexIndex + 3] * ColorTint;

                if (alpha == 0)
                {
                    startingCharacterRange += 1;

                    if (startingCharacterRange == characterCount)
                    {
                        // Update mesh vertex data one last time.
                        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    }
                }
            }

            // Upload the changed vertex colors to the Mesh.
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (currentCharacter + 1 < characterCount) currentCharacter += 1;

            yield return new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
        }
    }
}

