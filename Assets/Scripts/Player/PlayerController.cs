using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    public Joystick joystick;
    [SerializeField] private CharacterController controller;

    [SerializeField] private float playerNormalSpeed;
    [SerializeField] private float playerSlowedSpeed;
    private float playerForwardSpeed;

    [SerializeField] private Vector2 clampedXLimit;

    [SerializeField] private float footstepTimer;
    private float lastFootstep;
    [Header("Minions")]
    [SerializeField] private MinionController minionPrefab;
    [SerializeField] private TMP_Text minionCountText;
    [Range(0f, 1f)] [SerializeField] private float radius;
    [Range(0f, 1f)] [SerializeField] private float angle;
    private List<MinionController> Minions = new List<MinionController>();

    [Header("Stacking")] 
    [SerializeField] private float stackingSpace;
    [SerializeField] private float minionHeight;

    private void Start()
    {
        for (int i = 0; i < GameManager.Instance.startingMinionAmount; i++)
        {
            SpawnMinion();
        }

        playerForwardSpeed = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.isFinished)
        {
            return;
        }
        SetMinionPositions();
        Movement();
    }
    

    private void Movement()
    {
        var dir = new Vector3(joystick.Horizontal, 0, 1);
        controller.Move(dir * (playerForwardSpeed * Time.deltaTime));

        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, clampedXLimit.x, clampedXLimit.y);
        transform.position = pos;

        if (lastFootstep <= 0 && controller.velocity.magnitude > 0)
        {
            AudioManager.Instance.PlaySound2D("Footsteps");
            lastFootstep = footstepTimer;
        }
        else
        {
            lastFootstep -= Time.deltaTime;
        }
    }

    public void SpawnMinion()
    {
        var minion = LeanPool.Spawn(minionPrefab, transform.position, Quaternion.identity, transform);
        AddMinionsToList(minion);
        
        int currentMinionIndex = Minions.IndexOf(minion);
        minion.transform.position = GetMinionSpawnPosition(currentMinionIndex);
        minion.transform.localEulerAngles = Vector3.zero;

        minion.gameObject.name = "Minion " + currentMinionIndex;
        
        Debug.Log("Spawned minion");
    }

    private void SetMinionPositions()
    {
        float goldenAngle = 137.5f * angle;  

        for (int i = 0; i < Minions.Count; i++)
        {
            float x = radius * Mathf.Sqrt(i+1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i+1));
            float z = radius * Mathf.Sqrt(i+1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i+1));

            Vector3 minionLocalPosition = new Vector3(x, 0, z);
            Vector3 minionTargetPosition = transform.TransformPoint(minionLocalPosition);

            var currentMinion = Minions[i];

            Vector3 velocityDirection = minionTargetPosition - currentMinion.transform.position;
            velocityDirection.y = 0;
            currentMinion.GetRigidBody().velocity = Vector3.Lerp(currentMinion.GetRigidBody().velocity, velocityDirection, 0.1f);

            //transform.GetChild(i).localPosition = Vector3.Lerp(transform.GetChild(i).localPosition, runnerLocalPosition, 0.1f);
        }
    }
    
    private Vector3 GetMinionSpawnPosition(int index)
    {
        float goldenAngle = 137.5f * angle;  

        float x = radius * Mathf.Sqrt(index + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (index + 1));
        float z = radius * Mathf.Sqrt(index + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (index + 1));

        Vector3 localMinionPosition = new Vector3(x, 0, z);
        Vector3 minionTargetPosition = transform.TransformPoint(localMinionPosition);

        return minionTargetPosition;
    }

    [ContextMenu("Stack Minions")]
    public void StackMinions(GameObject stackingCamera)
    {
        var pos = transform.position;
        pos.x = 0;
        transform.DOMove(pos, 0.25f);
        
        float floatLineCount = (-1 + Mathf.Sqrt(1 + 8 * Minions.Count)) / 2;
        int lineCount = Mathf.FloorToInt(floatLineCount);
        int minionInFirstLine = Mathf.CeilToInt(floatLineCount);
        
       StartCoroutine(StackMinionsCO(lineCount, minionInFirstLine, stackingCamera));

    
    }

    public void FightBoss(BossController boss)
    {
        foreach (MinionController minion in Minions)
        {
            minion.FightBoss(boss);
        }
    }

    private IEnumerator StackMinionsCO(int lineCount, int minionInFirstLine, GameObject stackingCamera)
    {
        for (int i = 0; i < Minions.Count; i++)
        {
            int minionLine = 0;
            int minionIndex = i;

            for (int j = 0; j < lineCount; j++)
            {
                minionIndex -= (minionInFirstLine - j);

                if (minionIndex < 0)
                    break;

                minionLine++;
            }

            int previousMinionIndex = 0;

            for (int k = 0; k < minionLine; k++)
                previousMinionIndex += minionInFirstLine - k;


            int currentMinionLine = minionInFirstLine - minionLine;
            int minionIndexInCurrentLine = i - previousMinionIndex;


            float minionPositionX = -(currentMinionLine / 2f) * stackingSpace + stackingSpace / 2 +
                                    stackingSpace * minionIndexInCurrentLine;
            float minnionPositionY = minionLine * minionHeight;

            Vector3 minionTargetPosition = new Vector3(minionPositionX, minnionPositionY, 0);
            Minions[i].transform.DOLocalMove(minionTargetPosition, 0.2f).SetEase(Ease.Flash); //= minionTargetPosition;
            Minions[i].GetRigidBody().isKinematic = true;
            yield return new WaitForSeconds(0.02f);
        }
        
        foreach (MinionController minion in Minions)
        {
            minion.GetReadyToStack();
        }
        stackingCamera.SetActive(true);
    }

    public CharacterController GetController()
    {
        return controller;
    }

    public void StartGame()
    {
        playerForwardSpeed = playerNormalSpeed;
        minionCountText.gameObject.SetActive(true);
    }

    public void AddMinionsToList(MinionController minionToAdd)
    {
        if (Minions.Contains(minionToAdd))
        {
            return;
        }
        Minions.Add(minionToAdd);
        minionCountText.SetText(CurrentMinionCount().ToString());
    }
    
    public void RemoveMinionsFromList(MinionController minionToRemove)
    {
        if (!Minions.Contains(minionToRemove))
        {
            return;
        }
        minionToRemove.transform.SetParent(null);
        Minions.Remove(minionToRemove);
        
        minionCountText.SetText(CurrentMinionCount().ToString());

        if (Minions.Count <= 0)
        {
            GameManager.Instance.FailLevel();
            minionCountText.gameObject.SetActive(false);
        }


    }

    public int CurrentMinionCount()
    {
        return Minions.Count;
    }

    public List<MinionController> MyMinions()
    {
        return Minions;
    }

    public void SetPlayerSpeed(bool isSlowed)
    {
        playerForwardSpeed = isSlowed ? playerSlowedSpeed : playerNormalSpeed;

        foreach (MinionController minion in Minions)
        {

            minion.GetAnimator().speed = isSlowed ? 0.25f : 1;
        }
    }

    public bool AreAllMinionsFinished()
    {
        foreach (MinionController minion in Minions)
        {
            if (!minion.IsFinishedStair())
            {
                return false;
            }
        }

        return true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate"))
        {
            other.GetComponent<Gate>().TriggerGate(this);
        }

        if (other.CompareTag("EnemyTile"))
        {
            SetPlayerSpeed(true);
        }

        if (other.CompareTag("FinishLine"))
        {
            other.GetComponent<FinishManager>().TriggerFinish(this);
            minionCountText.gameObject.SetActive(false);
            GameManager.Instance.WinLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyTile"))
        {
            SetPlayerSpeed(false);
        }    
    }
}
