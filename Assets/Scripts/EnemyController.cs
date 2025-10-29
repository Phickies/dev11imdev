using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    public ParticleSystem hitParticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit()
    {
        ComboManager.instance.AddCombo();

        //particle
        ParticleSystem ps = Instantiate(hitParticle, transform.position , Quaternion.identity);
        var psmain = ps.main;
        int level = ComboManager.instance.GetComboLevel();
        psmain.startColor = LvlToClr(level);
        float size = level * 0.15f + 0.1f;
        Debug.Log(size);
        psmain.startSize = size;
        ps.Play();
        Destroy(ps.gameObject, ps.main.startLifetime.constant);
       
    }

    private Color LvlToClr(int lvl)
    {
        switch (lvl)
        {
            case 1: return Color.white;
            case 2: return Color.cyan;
            case 3: return Color.blue;
            case 4: return new Color(205f/255f, 127f/255f, 50f/255f);//bronze
            case 5: return new Color(192f / 255f, 192f / 255f, 192f / 255f);//silver
            case 6: return new Color(255f / 255f, 215f / 255f, 0f / 255f);//gold
            default: return Color.white;
        }
    }
}
