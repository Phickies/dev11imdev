using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return null;
                }

                if (instance == null)
                {
                    Instantiate(Resources.Load<GameManager>("GameManager"));
                }
#endif
                return instance;
            }
        }
        [Header("References")]
        public PlayerManager playerManager;
        public ComboManager comboMan;
        public SpawnManager spawnManager;
        public CardManager cardman;

        [Header("Boss Rave Settings")]
        [SerializeField] private float bossRaveDuration = 30f;
        [SerializeField] private GameObject bossDefeatedBoxPrefab;
        [SerializeField] private Vector3 bossDefeatedBoxSpawnPoint;

        private Coroutine bossRaveRoutine;
        private bool bossRaveTriggered;

        [Header("Gameplay Settings")]
        public float normalGravity = -30.24f;
        [SerializeField] private float gravity = -30.24f;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (playerManager == null)
            {
                playerManager = Object.FindFirstObjectByType<PlayerManager>();
                Debug.Log("found player");
                if (playerManager == null)
                {
                    Debug.Log("no player found");
                }
            }
            if (comboMan == null)
            {
                comboMan = Object.FindFirstObjectByType<ComboManager>();
                Debug.Log("found combomanager");
                if (comboMan == null)
                {
                    Debug.Log("no combo found");
                }
            }
            if (spawnManager == null)
            {
                spawnManager = Object.FindFirstObjectByType<SpawnManager>();
                Debug.Log("found spawnmanager");
                if (spawnManager == null)
                {
                    Debug.Log("no sapwn found");
                }
            }
            if (cardman == null)
            {
                cardman = Object.FindFirstObjectByType<CardManager>();
                Debug.Log("found card");
                if (cardman == null)
                {
                    Debug.Log("no player found");
                }
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                SaveSystem.Save();
                Debug.Log("Saved game");
            }
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                SaveSystem.Load();
                Debug.Log("Game loaded");
            }
        }

        public float GetGravity()
        {
            return gravity;
        }
        
        public void UpdateGravity(float value)
        {
            gravity = value;
        }

        public void ResetGravity()
        {
            gravity = normalGravity;
        }
        
        public void StartBossRave()
        {
            if (bossRaveTriggered)
            {
                return;
            }

            bossRaveTriggered = true;

            if (bossRaveRoutine != null)
            {
                StopCoroutine(bossRaveRoutine);
            }

            bossRaveRoutine = StartCoroutine(BossRaveTimer());
        }

        private IEnumerator BossRaveTimer()
        {
            float remainingTime = bossRaveDuration;
            Debug.Log("Boss Rave started");

            while (remainingTime > 0f)
            {
                if (playerManager != null && playerManager.IsDead())
                {
                    bossRaveRoutine = null;
                    yield break;
                }

                remainingTime -= Time.deltaTime;
                Debug.Log("Boss Rave time left: " + remainingTime);
                yield return null;
            }

            if (playerManager != null && playerManager.IsDead())
            {
                bossRaveRoutine = null;
                yield break;
            }

            SpawnBossDefeatedBox();
            bossRaveRoutine = null;
        }

        private void SpawnBossDefeatedBox()
        {
            if (GameObject.FindGameObjectWithTag("Defeated boss") != null)
            {
                return;
            }

            if (bossDefeatedBoxPrefab == null)
            {
                Debug.LogWarning("Boss defeated box prefab is not assigned in GameManager.");
                return;
            }

            if (bossDefeatedBoxSpawnPoint == null)
            {
                Debug.LogError("Boss defeated box spawn point is not assigned in GameManager.");
                return;
            }

            GameObject defeatedBox = Instantiate(
                bossDefeatedBoxPrefab,
                bossDefeatedBoxSpawnPoint,
                Quaternion.identity);
            defeatedBox.tag = "Defeated boss";
        }
    }
}
