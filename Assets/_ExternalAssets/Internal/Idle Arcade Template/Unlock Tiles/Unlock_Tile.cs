using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Lean.Pool;
using UnityEngine.UI;


namespace LaynarGames.IdleArcadeTemplate
{
    public class Unlock_Tile : MonoBehaviour
    {
        [SerializeField] GameObject unlockCanvas;
        [SerializeField] GameObject unlockObject;
        [SerializeField] GameObject[] deactivateOnUnlock;

        [SerializeField] bool overrideUnlock;
        [SerializeField] bool overrideUnlockValue;
        [SerializeField] int value;
        int originalValue;
        [SerializeField] TextMeshProUGUI valueText;

        [SerializeField] BoxCollider coll;
        Coroutine paymentCR;

        [SerializeField] int cashCollected;
        bool alreadyPurchased;

        public string ID;

        public UnityEvent eventOnUnlock;
        public UnityEvent eventOnOverrideUnlock;

        [SerializeField] Transform spawnParticlePos;
        [SerializeField] GameObject buyParticles;

        [SerializeField] bool dontPerformScaleAnimation;

        [SerializeField] Image fillImage;
        float fillAmount;

        [SerializeField] GameObject moneyPrefab;
        [SerializeField] GameObject payParticle;

        private void Awake()
        {
            originalValue = value;
            valueText.text = value.ToString();   
            
            if (overrideUnlock)
            {
                if (overrideUnlockValue)
                {
                    alreadyPurchased = true;
                }
            }

            if (!overrideUnlock)
            {
                if (PlayerPrefs.HasKey(("Buy_Price_Remaining" + ID)))
                {
                    value = PlayerPrefs.GetInt(("Buy_Price_Remaining" + ID));
                    valueText.text = value.ToString();
                }

                if (PlayerPrefs.HasKey(("Buy_Area_Purchased" + ID)))
                {
                    alreadyPurchased = bool.Parse(PlayerPrefs.GetString("Buy_Area_Purchased" + ID));
                }
            }

            if (alreadyPurchased)
            {
                if(unlockObject != null){unlockObject.gameObject.SetActive(true);}
                
                unlockCanvas.gameObject.SetActive(false);
                if (eventOnOverrideUnlock != null)
                {
                    eventOnOverrideUnlock.Invoke();
                }

                if(deactivateOnUnlock.Length > 0)
                {
                    for (int i = 0; i < deactivateOnUnlock.Length; i++)
                    {
                        deactivateOnUnlock[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (unlockObject != null)
                {
                    unlockObject.gameObject.SetActive(false);
                }
            }
        }



        public void StartPaying()
        {
            if (alreadyPurchased) { return; }
            paymentCR = StartCoroutine(PayForTile());
        }

        IEnumerator PayForTile()
        {
            yield return new WaitForSeconds(.1f);

            float timeElapsed = 1;

            while (true)
            {
                if(cashCollected <= 0) { yield break; }

                bool reachedZero = false;

                int amountDeductedThisTick = Mathf.RoundToInt(timeElapsed * (timeElapsed * 5f));

                if(cashCollected - amountDeductedThisTick > 0)
                {
                    cashCollected -= amountDeductedThisTick;
                } else
                {
                    cashCollected = 0;
                    reachedZero = true;
                }

                if (value - amountDeductedThisTick > 0)
                {
                    value -= amountDeductedThisTick;

                    //GameObject money = LeanPool.Spawn(moneyPrefab, PlayerTopDownController.instance.moneyTarget.position, Quaternion.identity, null);
                    //money.transform.DOMove(transform.position, .2f).SetEase(Ease.Linear).OnComplete(SpawnPayParticle);
                    //LeanPool.Despawn(money, .2f);
                    //TODO: SPAWN MONEY FROM PLAYER TO TILE
                }
                else
                {
                    int difference = amountDeductedThisTick - value;
                    cashCollected += difference;

                    value = 0;
                    reachedZero = true;

                    Unlock();
                }

                valueText.text = value.ToString();
                UpdateFill();

                if (reachedZero)
                {
                    yield break;
                }

                yield return new WaitForSeconds(.1f);
                timeElapsed += .1f;
            }
        }

        void SpawnPayParticle()
        {
            GameObject particle = LeanPool.Spawn(payParticle, transform.position, Quaternion.identity, null);
            LeanPool.Despawn(particle, .3f);
        }

        public void StopPaying()
        {
            if(paymentCR != null)
            {
                StopCoroutine(paymentCR);
            }
        }

        void Unlock()
        {
            alreadyPurchased = true;
            
            if (unlockObject != null)
            {
                unlockObject.gameObject.SetActive(true);
                
                if (!dontPerformScaleAnimation)
                {
                    unlockObject.transform.DOPunchScale(Vector3.one * 1.5f, .3f, 3, .5f);
                }
            }

            unlockCanvas.gameObject.SetActive(false);
            PlayerPrefs.SetString("Buy_Area_Purchased" + ID, "true");

            if(eventOnUnlock != null)
            {
                eventOnUnlock.Invoke();
            }

            GameObject buyParticlesGO = LeanPool.Spawn(buyParticles, spawnParticlePos.position, Quaternion.identity, null);
            LeanPool.Despawn(buyParticlesGO, 3);

            if (deactivateOnUnlock.Length > 0)
            {
                for (int i = 0; i < deactivateOnUnlock.Length; i++)
                {
                    deactivateOnUnlock[i].gameObject.SetActive(false);
                }
            }

            //HomaBelly.Instance.TrackDesignEvent("Unlocked Tile");

            if (dontPerformScaleAnimation)
            {
                //HomaBelly.Instance.TrackDesignEvent("Unlocked New Area");
            }
        }

        void UpdateFill()
        {
            float amount = (100 - ((((float)value / (float)originalValue) * 100)));
            fillAmount = amount * .01f;
            fillImage.DOKill();
            fillImage.DOFillAmount(fillAmount, .1f);   
        }
    }
}

