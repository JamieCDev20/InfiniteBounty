using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PiggyBank : SubjectBase, IInteractible
{
    [SerializeField] private int i_targetAmount;
    [SerializeField] private int i_inputAmount;
    [SerializeField] private int i_storedAmount;
    [SerializeField] GameObject go_portal;
    private DifficultyManager dm_man;
    [SerializeField] private TextMeshPro tmp_currentMoneyText;
    [SerializeField] private GameObject go_stand;
    private AudioSource as_source;
    private Quaternion q_startRot;
    private Vector3 v_startPos;
    private float f_currentTimer;
    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        as_source = GetComponent<AudioSource>();

        v_startPos = transform.position;
        q_startRot = transform.rotation;

        //if (DifficultyManager.x.MaximumDifficulty < 9)
        //    gameObject.SetActive(false);

        SaveManager _sm = FindObjectOfType<SaveManager>();
        i_storedAmount = _sm.SaveData.i_zippyBank;
        if (i_storedAmount != 0)
            tmp_currentMoneyText.text = "£" + i_storedAmount;
        AddObserver(_sm);
        transform.localScale = Vector3.one + Vector3.one * (i_storedAmount * 0.000001f);
    }

    private void Update()
    {
        if (rb.velocity == Vector3.zero || transform.position.y < -100)
        {
            f_currentTimer += Time.deltaTime;

            if (f_currentTimer > 5)
            {
                transform.position = v_startPos;
                transform.rotation = q_startRot;
            }
        }
        else
            f_currentTimer = 0;
    }

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (interactor?.GetComponent<NugManager>().Nugs >= i_inputAmount)
        {
            as_source.Play();
            NugManager nugMan = interactor.GetComponent<NugManager>();
            nugMan.GetComponent<NugManager>().CollectNugs(-i_inputAmount, false);
            i_storedAmount += i_inputAmount;
            SaveEvent _se = new SaveEvent(new PlayerSaveData(nugMan.Nugs, -1, i_storedAmount, null, null, null, null, null, null, -1));
            Notify(_se);
            transform.localScale = Vector3.one + Vector3.one * (i_storedAmount * 0.000001f);
            tmp_currentMoneyText.text = "£" + i_storedAmount;

            if (i_storedAmount >= i_targetAmount)
                EnablePortal();
        }

        TutorialManager.x.InteractedWithZippyBack();
    }

    public void SetInputAmount(int _input)
    {
        i_inputAmount = _input;
    }

    private void EnablePortal()
    {
        go_portal.SetActive(true);
        go_portal.transform.parent = null;
        go_portal.transform.localScale = Vector3.one;

    }
}
