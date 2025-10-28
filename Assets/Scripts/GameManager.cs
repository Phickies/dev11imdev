using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float gravity = -30.24f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public float GetGravity()
        {
            return gravity;
        }
        
        public void UpdateGravity(float value)
        {
            gravity = value;
        }
    }
}
