using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class StupidAssRotateThatTomWanted : MonoBehaviour
{
	//Most in finnish, but this is a code that was abandoned and completely rewritten. A good showcase of when one should just start over on a clean slate

    //Ohjain pystysuunnassa matchaa kaikki vastaavat x/z
    //Ohjain vaakasuunnassa matchaa kaikki Y
    // Use this for initialization

    private SteamVR_TrackedObject trackObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackObj.index); }
    }

    private void Awake()
    {
        trackObj = GetComponent<SteamVR_TrackedObject>();
    }

    #region Variables
    [Header("Variable stoofs")]
    public Vector3 startPoint;
    public float angle = 20f;
    private Vector3 dirRightHand;
    public LayerMask harashoo;
    public int layerInt;
    public float distancce;

    [Space]

    [Header("Script components")]
    private Hand handu;
    public UIGenerator uiGenComponent;
    public Generator genCompo;

    [Space]

    [Header("Transforms")]
    public Transform mapCubeParent;
    public Transform daddyObject;
    public Transform originalPlayerParent;

    [Space]

    [Header("GameObjects")]
    public GameObject mapGameObject;
    public GameObject controllerObj;

    [SerializeField]
    private GameObject pivotGameObject;
    public GameObject bigCubePivotGameObject;
    public GameObject playerGameObject;

    [Space]

    [Header("Lists")]
    public List<Transform> uiTransforms;
    public List<Transform> hitTransforms;
    public List<GameObject> uiGameObjects;
    public List<GameObject> genGameObjects;
    public int[] intList;

    [Space]

    [SerializeField]
    private bool distanceCheck;

    [SerializeField]
    private bool angleCheck;

    [SerializeField]
    private bool rotating;
    public bool playerInRotation;


    #endregion


    //Pivotilla on sama sijainti kuin kuution keskipisteillä......
    //Jos sisältää enemmän kuin 1 pivotpointtia = käytä keskipivottia....


    void Start()
    {
        //Other script components and the values we need
        genCompo = GameObject.Find("LevelGenerator").GetComponent<Generator>();
        uiGenComponent = GameObject.Find("Hand1").GetComponent<UIGenerator>();
        mapCubeParent = uiGenComponent.parentHolderUI.transform;
        daddyObject = genCompo.parentHolder.transform;
        uiGameObjects = uiGenComponent.comparisonGameObjectsGeneratorUI;
        genGameObjects = genCompo.comparisonGameObjectsGenerator;
        playerGameObject = GameObject.Find("VRStuff");

        //VR hand stuff
        handu = gameObject.GetComponent<Hand>();
        controllerObj = handu.controllerObject;

        //Declarations axaxaxaax
        uiTransforms = new List<Transform>();
        dirRightHand = transform.TransformDirection(Vector3.left);

        //Bools
        rotating = false;
        distanceCheck = false;
        angleCheck = false;
        playerInRotation = false;

        //Should make better
        layerInt = 12 << 8;
        intList = new int[3];


        //Get the cubes jaa
        foreach (Transform childTransform in mapCubeParent)
        {
            uiTransforms.Add(childTransform);
        }
        Debug.Log(uiTransforms.Count);
        mapCubeParent.gameObject.SetActive(false);
    }

    #region Update

    void Update()
    {
        dirRightHand = transform.TransformDirection(Vector3.left);
        var hits = Physics.RaycastAll(transform.position, dirRightHand, 10.0f, harashoo);
        Debug.DrawRay(transform.position, dirRightHand, Color.red);

        if (hits.Length != 0)
        {
            hitTransforms = new List<Transform>();

            if (hits.Length >= 2)
            {
                BoolChecks(hits);
                AxisCheck(hits);
                foreach (var t in hits)
                {
                    var tempHitTransform = t.transform;
                    hitTransforms.Add(tempHitTransform);
                }
            }

            if (!handu.GetStandardApplicationMenuButton() || !distanceCheck || !angleCheck || rotating)
            {
                return;
            }
            PivotCreation(intList, hits);
            Debug.Log("Logic works!");

            hitTransforms.Clear();
        }

        /*if (bigCubePivotGameObject != null && pivotGameObject != null)
        {
            bigCubePivotGameObject.transform.rotation = pivotGameObject.transform.rotation;
        }*/
    }

    #endregion

    #region Awful stuff (pivotCreation)
    private void PivotCreation(int[] ints, RaycastHit[] rayHitObjects)
    {
        var xInt = ints[0];
        var yInt = ints[1];
        var zInt = ints[2];

        if (xInt != 0 || yInt != 0 || zInt != 0)
        {
            var rayTransformOne = rayHitObjects[0].transform.localPosition;
            var pivotPos = new Vector3(0, 0, 0);
            var cubeList = new List<Transform>();
            var bigCubeList = new List<Transform>();
            var xValue = rayTransformOne.x;
            var zValue = rayTransformOne.z;
            /* var xAll = 0.0f;
             var yAll = 0.0f;
             var zAll = 0.0f;*/
            var uiSideLengths = new[] { 0f, 0f, 0f };
            var cubeSideLengths = new[] { 0f, 0f, 0f };
            var uiCubeIndex = 0;

            pivotGameObject = Instantiate(genCompo.cubes[4], pivotPos, Quaternion.identity, mapCubeParent);
            bigCubePivotGameObject = Instantiate(genCompo.cubes[4], pivotPos, Quaternion.identity, daddyObject);

            if (zInt > xInt)
            {
                foreach (var childCube in uiTransforms)
                {
                    if (Math.Abs(zValue - childCube.localPosition.z) < 0.001f)
                    {
                        bigCubeList.Add(genGameObjects[uiCubeIndex].transform);
                        Debug.Log("bigCubelist countX = " + bigCubeList.Count);
                        cubeList.Add(childCube);
                        /* xAll += childCube.localPosition.x;
                         yAll += childCube.localPosition.y;
                         zAll += childCube.localPosition.z;*/
                        uiSideLengths[0] += childCube.localPosition.x;
                        uiSideLengths[1] += childCube.localPosition.y;
                        uiSideLengths[2] += childCube.localPosition.z;
                        cubeSideLengths[0] += genGameObjects[uiCubeIndex].transform.localPosition.x;
                        cubeSideLengths[1] += genGameObjects[uiCubeIndex].transform.localPosition.y;
                        cubeSideLengths[2] += genGameObjects[uiCubeIndex].transform.localPosition.z;

                        var cubeHolder = genGameObjects[uiCubeIndex].gameObject;
                        var playerBool = cubeHolder.GetComponentInChildren<playerContactTrigger>().isPlayerInThisCube;

                        if (playerBool)
                        {
                            playerInRotation = true;
                        }
                    }

                    uiCubeIndex++;
                }
                Debug.Log(playerInRotation);
                Debug.Log("Cubesidelength list for the big one " + cubeSideLengths[0] + " " + cubeSideLengths[1] + " " + cubeSideLengths[2]);

                pivotGameObject.transform.localPosition = new Vector3(uiSideLengths[0] / (genCompo.widthNumber * genCompo.heightNumber), uiSideLengths[1] / (genCompo.depthNumber * genCompo.widthNumber), uiSideLengths[2] / (genCompo.heightNumber * genCompo.depthNumber));

                var bigCubeRoundedX = Mathf.Round(cubeSideLengths[0] / (genCompo.widthNumber * genCompo.heightNumber) * 10f) / 10f;
                var bigCubeRoundedY = Mathf.Round(cubeSideLengths[1] / (genCompo.depthNumber * genCompo.widthNumber) * 10f) / 10f;
                var bigCubeRoundedZ = Mathf.Round(cubeSideLengths[2] / (genCompo.heightNumber * genCompo.depthNumber) * 10f) / 10f;
                bigCubePivotGameObject.transform.localPosition = new Vector3(bigCubeRoundedX, bigCubeRoundedY, bigCubeRoundedZ);

                foreach (var mapCubeChildren in cubeList)
                {
                    mapCubeChildren.SetParent(pivotGameObject.transform);
                }

                foreach (var bigCubeKids in bigCubeList)
                {
                    bigCubeKids.SetParent(bigCubePivotGameObject.transform);
                }
                /* if (playerInRotation)
                 {
                     playerGameObject.transform.SetParent(bigCubePivotGameObject.transform);
                 }*/

                var pivotArray = new[] { pivotGameObject.transform, bigCubePivotGameObject.transform };

                StartCoroutine(RotateMe(pivotArray, Vector3.forward, 90f));
            }

            else
            {
                Debug.Log("Gusta ja paskaa rotatio selectit ei toiminu");
            }
            //Has been changed
            bigCubeList.Clear();
            cubeList.Clear();
        }
        else
        {
            Debug.Log("No hits in lists");
        }
    }


    #endregion

    #region AxisCheck
    private void AxisCheck(IEnumerable<RaycastHit> raycastHits)
    {
        var floatsx = new List<float>();
        var floatsy = new List<float>();
        var floatsz = new List<float>();

        foreach (var t in raycastHits)
        {
            var tempVector = t.transform.localPosition;

            floatsx.Add(tempVector.x);
            floatsy.Add(tempVector.y);
            floatsz.Add(tempVector.z);
        }

        var newFloatsX = floatsx.FindAll(s => s.Equals(floatsx[0]));
        var newFloatsY = floatsy.FindAll(t => t.Equals(floatsy[0]));
        var newFloatsZ = floatsz.FindAll(u => u.Equals(floatsz[0]));

        intList = new[] { newFloatsX.Count, newFloatsY.Count, newFloatsZ.Count };
    }
    #endregion

    #region BoolChecks
    private void BoolChecks(IList<RaycastHit> rayHits)
    {
        if (rayHits[0].distance < distancce)
        {
            distanceCheck = true;

            angleCheck = Vector3.Angle(transform.position, rayHits[0].transform.position) < angle;
        }
        else
        {
            distanceCheck = false;
        }
    }
    #endregion

    #region CleanUpRotation
    private static void CleanUpRotation(GameObject pivotObject)
    {
        Destroy(pivotObject);
    }
    #endregion



    #region Rotation
    private IEnumerator RotateMe(Transform[] pivots, Vector3 axis, float turnAngle)
    {
        rotating = true;
        var from = pivots[0].localRotation;
        var to = pivots[0].localRotation;
        to *= Quaternion.Euler(axis * turnAngle);

        var elapsed = 0.0f;
        while (elapsed < 1)
        {
            pivots[0].localRotation = Quaternion.Slerp(from, to, elapsed / 1);
            pivots[1].localRotation = Quaternion.Slerp(from, to, elapsed / 1);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pivots[0].rotation = to;

        var roundRot = new Quaternion(Mathf.Round(pivots[0].localRotation.x), Mathf.Round(pivots[0].localRotation.y),
            Mathf.Round(pivots[0].localRotation.z), Mathf.Round(pivots[0].localRotation.w));

        var roundRot2 = new Quaternion(Mathf.Round(pivots[1].localRotation.x), Mathf.Round(pivots[1].localRotation.y),
            Mathf.Round(pivots[1].localRotation.z), Mathf.Round(pivots[1].localRotation.w));

        pivots[0].localRotation = roundRot;
        pivots[1].localRotation = roundRot2;

        for (var i = 0; i < mapCubeParent.childCount; i++)
        {
            if (mapCubeParent.GetChild(i).tag != "Pivot") continue;
            for (var j = 0; j < mapCubeParent.GetChild(i).childCount; j++)
            {
                mapCubeParent.GetChild(i).GetChild(j).SetParent(mapCubeParent);
                j--;
            }
        }


        // playerGameObject.transform.parent = null;

        CleanUpRotation(pivotGameObject);
        CleanUpRotation(bigCubePivotGameObject);
        playerInRotation = false;

        yield return rotating = false;
    }
    #endregion
}

