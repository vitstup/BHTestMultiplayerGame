using UnityEngine;
using Mirror;

public class PlayerDamagable : NetworkBehaviour, IDamagable
{
    [SerializeField] private float invulnerabilityTime = 3f;

    private MeshRenderer[] renderers;
    [SyncVar(hook = nameof(OnInvulnerabilityTimeTimerChanged))] private float invulnerabilityTimeTimer;
    [SyncVar(hook = nameof(OnInvulnvulnerabilityColorChanged))] private Color invulnerabilityColor;
    private Color[] baseColors;

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        GetBaseColors();
    }

    private void Update()
    {
        if (isServer && !DamagableNow())
        {
            invulnerabilityTimeTimer -= Time.deltaTime;
        }

        if (!DamagableNow())
        {
            if (renderers[0].material.color != invulnerabilityColor)
            {
                // Apply color change directly on the server
                ChangeColor(invulnerabilityColor);
            }
        }
        else
        {
            if (renderers[0].material.color != baseColors[0])
            {
                // Apply color change directly on the server
                ChangeColor(baseColors);
            }
        }
    }

    public void TakeDamage()
    {
        if (isServer)
        {
            invulnerabilityTimeTimer = invulnerabilityTime;
            Color color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            invulnerabilityColor = color;

            // Apply color change directly on the server
            ChangeColor(invulnerabilityColor);
        }
    }

    public bool DamagableNow()
    {
        return invulnerabilityTimeTimer <= 0;
    }

    private void ChangeColor(Color color)
    {
        foreach (var meshRenderer in renderers)
        {
            meshRenderer.material.color = color;
        }
    }

    private void ChangeColor(Color[] colors)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = colors[i];
        }
    }

    private void GetBaseColors()
    {
        baseColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].material.color;
        }
    }

    private void OnInvulnerabilityTimeTimerChanged(float oldTimer, float newTimer)
    {
        invulnerabilityTimeTimer = newTimer;
    }

    private void OnInvulnvulnerabilityColorChanged(Color oldColor, Color newColor)
    {
        invulnerabilityColor = newColor;

        // Apply color change directly on the server
        ChangeColor(invulnerabilityColor);
    }
}