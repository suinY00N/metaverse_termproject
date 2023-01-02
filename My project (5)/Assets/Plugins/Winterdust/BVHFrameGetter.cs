using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using Winterdust; // for BVH class


using System;

using System.IO;
public class BVHFrameGetter : MonoBehaviour
{
    
    // https://winterdust.itch.io/bvhimporterexporter

    public List<string> jointPaths = new List<string>(); // emtpy one
    public List<Transform> avatarCurrentTransforms = new List<Transform>(); // The transforms for Skeleton gameObject

    public Transform avatarRootTransform; // defined in inspector
    public string bvhFileName = "";
    public BVH bvh;
    public GameObject MIC;
    // bool isbvh = false;
    MInput MInput;
    // MInput MInput;

    public int frameNo =0;
    public double secondsPerFrame;
    public int    frameCount;
    public int curframecount;

     public GameObject skeletonGO;

    
    // public int boneCount;

    // public double secondsPerFrame;

   
    // public int frameCount;

   //All bones and their data. The first (and often only) root bone is always at index 0. A child bone will always have a higher index than its parent (if you modify the array manually make sure to follow this rule). Important: bones.Length can't be trusted, use boneCount instead
        
    //    public BVH.BVHBone[] allBones;

    //void Start()
    public void Awake()
    {

        Detectbvh();
        // MInput = GameObject.Find("MIC").GetComponent<MInput>();
        // Vector3 vector = this.bvh.allBones[0].localFramePositions[this.frameNo];

        // Quaternion quaternion = this.bvh.allBones[0].localFrameRotations[this.frameNo];
    }
    public void Start()
    {
        // GameObject.FindGameObjectsWithTag("MIC").GetComponent<MicrophoneInput>()
        MInput = MIC.GetComponent<MInput>();
        // Detectbvh();
        
    }
    void Detectbvh()
    {

       // Load BVH motion data to this.bvh.allBones
        if (this.bvhFileName == "")
        {
            Debug.Log(" bvhFileName should be set in the inspector");
            throw new InvalidOperationException("No  Bvh FileName is set.");

        }
        
        this.bvh = new BVH(bvhFileName, parseMotionData: true); // parse the bvh file and load the  hierarchy paths and  load motion data to this.bvh.allBones

        this.frameCount = bvh.frameCount;
        curframecount = bvh.frameCount;
        this.secondsPerFrame = bvh.secondsPerFrame; // 0.05f;
        // Sets to 20 fps
        Time.fixedDeltaTime = (float)this.secondsPerFrame;



        //GameObject skeletonGO = myBvh.makeDebugSkeleton();
        // This line creates an animated skeleton from the BVH instance, visualized as a stick figure.
        //  makeSkeleton() does the same thing, except it isn't visualized/animated by default.

        //public GameObject makeDebugSkeleton(bool animate = true, string colorHex = "ffffff", float jointSize = 1f, int frame = -1, bool xray = false, bool includeBoneEnds = true, string skeletonGOName = "Skeleton", bool originLine = false)

       // Create a Skeleton hiearchy if it  is not yet created. If it exists, it implies that Skeleton was added to the scene
       // from the fbx varient of the Skeleton Prefab which was created by command "Convert to FBX Prefab variant" from GameOjbect tab in the topbar of Unity

        this.skeletonGO =  GameObject.FindGameObjectWithTag("Skeleton_Avatar");
        if ( this.skeletonGO == null)
        {
           // If there is not gameObject named "Skeleton:, create a skeleton of transforms for the skeleton hierarchy.
            this.skeletonGO = this.bvh.makeDebugSkeleton(animate:false, skeletonGOName: "Skeleton_Avatar"); // => if animate = false, dot not create an animation clip but only the rest pose; 
                                                                                                // Create BvhDebugLines component and MeshRenderer component,
                                                                                                //  but do not create Animation component
                                                                                                // public GameObject makeDebugSkeleton(bool animate = true, string colorHex = "ffffff", float jointSize = 1f, int frame = -1, bool xray = false, bool includeBoneEnds = true, string skeletonGOName = "Skeleton", bool originLine = false)

            Debug.Log(" bvh Skeleton is  created");
            this.avatarRootTransform = this.skeletonGO.transform.GetChild(0); // the Hips joint: The first child of SkeletonGO

            this.ParseAvatarRootTransform(this.avatarRootTransform, this.jointPaths, this.avatarCurrentTransforms);

            Debug.Log(" bvhFile has been read in Awake() of BVHFrameGetter");

        }
        else
        {
            Debug.Log(" bvh Skeleton is already created and has been given Tag 'Skeleton' ");


            //boneTransform.localPosition = this.localRestPosition;
            //boneTransform.localRotation = Quaternion.identity;

            // set the rest pose of this.bvh to this.skeletonGO:

            this.avatarRootTransform = this.skeletonGO.transform.GetChild(0); // the Hips joint: The first child of SkeletonGO

           

            //this.skeletonGO = this.bvh.makeDebugSkeleton(this.skeletonGO, this.avatarCurrentTransforms, animate: false);

            this.skeletonGO = this.bvh.makeDebugSkeleton(this.skeletonGO, animate: false);

            //this.bvh.makeDebugSkeleton(this.avatarRootTransform, this.avatarCurrentTransforms, animate: false); //this.skeletonGO => this.avatarRootTransform

            // Collect the transforms in the skeleton hiearchy into a list of transforms,  this.avatarCurrentTransforms:
            // If you change  this.avatarCurrentTransforms, it affects the hierarchy of    this.skeletonGO , because both reference the same transforms
            this.ParseAvatarRootTransform(this.avatarRootTransform, this.jointPaths, this.avatarCurrentTransforms);


            // Set the skeleton to the T-pose, the rest pose

        }


        // AnimationClip clip = myBvh.makeAnimationClip();
        //==>  This line just creates an AnimationClip. By default it has legacy set to true.
        //   But you can turn that off (either later or directly in the method call).

        //   public GameObject makeSkeleton(int frame = -1, bool includeBoneEnds = true, string skeletonGOName = "Skeleton", bool animate = false)
        // calls:  AnimationClip clip = this.makeAnimationClip(0, -1, false, "", WrapMode.Loop, true, false, false);

        // It's possible to make an Animator-compatible AnimationClip, which you can use in Mecanim via an Animator Override Controller.

        // BvhImporterExporter is delivered as a .dll file, everything is well documented with detailed XMLDOC descriptions.
        //  A .xml file is included, when placed next to the .dll the documentation can be seen from inside your script editor.

        // Most of the heavy work (actually importing the .bvh file) can be executed from a different thread if you want to make a preloader for your game. 
        // BvhImporterExporter has been optimized so it's blazingly fast, able to import thousands of animation frames in a very short time.

        // Get the transform sequence from Skeleton gameObject

        //this.avatarRootTransform = this.skeletonGO.transform.GetChild(0); // the Hips joint: The first child of SkeletonGO

        //this.ParseAvatarRootTransform(this.avatarRootTransform, this.jointPaths, this.avatarCurrentTransforms);

        // Debug.Log(" bvhFile has been read in Awake() of BVHFrameGetter");
        // => this.avatarCurrentTransforms holds the transforms of the skeleton hiearchy
    } // Awake()



    void ParseAvatarTransformRecursive(Transform child, string parentPath, List<string> jointPaths, List<Transform> transforms)
    {
        string jointPath = parentPath.Length == 0 ? child.gameObject.name : parentPath + "/" + child.gameObject.name;
        // The empty string's length is zero

        jointPaths.Add(jointPath);
        transforms.Add(child);

        foreach (Transform grandChild in child)
        {
            ParseAvatarTransformRecursive(grandChild, jointPath, jointPaths, transforms);
        }

        // Return if child has no children, that is, it is a leaf node.
    }

    void ParseAvatarRootTransform(Transform rootTransform, List<string> jointPaths, List<Transform> avatarTransforms)
    {
        jointPaths.Add(""); // The name of the root tranform path is the empty string
        avatarTransforms.Add(rootTransform);

        foreach (Transform child in rootTransform) // rootTransform class implements IEnuerable interface
        {
            ParseAvatarTransformRecursive(child, "", jointPaths, avatarTransforms);
        }
    }

    // Update is called once per frame
// MonoBehaviour.FixedUpdate has the frequency of the physics system; it is called every fixed frame-rate frame. Compute Physics system calculations after FixedUpdate.
//  0.02 seconds (50 calls per second) is the default time between calls. Use Time.fixedDeltaTime to access this value. 
//  Alter it by setting it to your preferred value within a script, or, navigate to Edit > Settings > Time > Fixed Timestep and set it there.
//   The FixedUpdate frequency is more or less than Update. If the application runs at 25 frames per second (fps), 
//   Unity calls it approximately twice per frame, Alternatively, 100 fps causes approximately two rendering frames with one FixedUpdate.
//  Control the required frame rate and Fixed Timestep rate from Time settings. Use Application.targetFrameRate to set the frame rate.

// Application.targetFrameRate: Specifies the frame rate at which Unity tries to render your game.
// Both Application.targetFrameRate and QualitySettings.vSyncCount let you control your game's frame rate for smoother performance. 
//targetFrameRate controls the frame rate by specifying the number of frames your game tries to render per second, whereas vSyncCount specifies the number of screen refreshes to allow between frames.


// On all other platforms, Unity ignores the value of targetFrameRate if you set vSyncCount. When you use vSyncCount, Unity calculates the target frame rate by dividing the platform's default target frame rate by the value of vSyncCount.
 //For example, if the platform's default render rate is 60 fps and vSyncCount is 2, Unity tries to render the game at 30 frames per second.

    //public void GetCurrentFrame(List<Transform> avatarCurrentTransforms)
    // public void GetCurrentFrame()
    void FixedUpdate()
    {


    if (this.frameNo == 0)
    {
        // GetComponent("MicrophoneInput").PlayRecordedAudio();
        // .PlayRecordedAudio();
        MInput.PlayRecordedAudio();
    }


    if (this.frameNo < this.bvh.frameCount)
    {

        for (int b = 0; b < bvh.boneCount; b++) // boundCount: 57 ordered in depth first search of the skeleton hierarchy: bvh.boneCount = 57
        // HumanBodyBones: Hips=0....; LastBone = 55
        {
            
            Quaternion quaternion = this.bvh.allBones[b].localFrameRotations[this.frameNo];
          
            avatarCurrentTransforms[b].localRotation = quaternion;  
               
        } //  for each bone

        // Increment frameNo for the next fixed update call
      
            this.frameNo++;
    }
        // check if the current frame number frameNo exceeds frameCount
        if (this.frameNo >= this.bvh.frameCount) // frameNo = 0 ~ 519;  frameCount = 520
        {
            this.frameNo = this.bvh.frameCount;
            newDetectbvh(); // go to the beginning of the frame

        }

    } 
    void newDetectbvh()
    {
        this.bvh = new BVH(bvhFileName, parseMotionData: true); // parse the bvh file and load the  hierarchy paths and  load motion data to this.bvh.allBones
        //this.frameCount = bvh.frameCount;

        if (bvh.frameCount != curframecount)
        {
            Detectbvh(); 
        }
    }

     //void FixedUpdate()
     //{
     //   GetCurrentFrame();
    // }

} // BVHTest class

