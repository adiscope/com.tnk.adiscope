/*
 * Referenced from:
 * https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread
 * 
 * Added by Hyunchang Kim (martin.kim@neowiz.com)
 */
#define ENABLE_UPDATE_FUNCTION_CALLBACK
//#define ENABLE_LATEUPDATE_FUNCTION_CALLBACK
//#define ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adiscope.Internal
{
    /// <summary>
    /// to invoke function must be run in unity's main thread from
    /// callbacks which is not in unity's main thread 
    /// </summary>
    internal class UnityThread : MonoBehaviour
    {
        /// singleton instance
        private static UnityThread instance = null;

        /// <summary>
        /// holds actions received from another Thread.
        /// will be coped to actionCopiedQueueUpdateFunc then executed from there
        /// </summary>
        private static List<Action> actionQueuesUpdateFunc = new List<Action>();

        /// <summary>
        /// holds Actions copied from actionQueuesUpdateFunc to be executed
        /// </summary>
        private List<Action> actionCopiedQueueUpdateFunc = new List<Action>();

        /// <summary>
        /// used to know if whe have new Action function to execute. 
        /// this prevents the use of the lock keyword every frame
        /// </summary>
        private volatile static bool noActionQueueToExecuteUpdateFunc = true;

#if (ENABLE_LATEUPDATE_FUNCTION_CALLBACK)
        /// <summary>
        /// Holds actions received from another Thread.
        /// Will be coped to actionCopiedQueueLateUpdateFunc then executed from there
        /// </summary>
        private static List<Action> actionQueuesLateUpdateFunc = new List<Action>();

        /// <summary>
        /// holds Actions copied from actionQueuesLateUpdateFunc to be executed
        /// </summary>
        private List<Action> actionCopiedQueueLateUpdateFunc = new List<Action>();

        /// <summary>
        /// Used to know if whe have new Action function to execute.
        /// This prevents the use of the lock keyword every frame
        /// </summary>
        private volatile static bool noActionQueueToExecuteLateUpdateFunc = true;
#endif

#if (ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK)
        /// <summary>
        /// holds actions received from another Thread.
        /// Will be coped to actionCopiedQueueFixedUpdateFunc then executed from there
        /// </summary>
        private static List<Action> actionQueuesFixedUpdateFunc = new List<Action>();

        /// <summary>
        /// holds Actions copied from actionQueuesFixedUpdateFunc to be executed
        /// </summary>
        private List<Action> actionCopiedQueueFixedUpdateFunc = new List<Action>();

        /// <summary>
        /// Used to know if whe have new Action function to execute.
        /// This prevents the use of the lock keyword every frame
        /// </summary>
        private volatile static bool noActionQueueToExecuteFixedUpdateFunc = true;
#endif

        /// <summary>
        /// used to initialize UnityThread. this must be executed in unity main thread.
        /// </summary>
        /// <param name="visible">UnityThread's game object visible in game scene/hierachy</param>
        public static void initUnityThread(bool visible = false)
        {
            if (instance != null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                // add an invisible game object to the scene
                GameObject obj = new GameObject("UnityThreadExecuter");
                if (!visible)
                {
                    obj.hideFlags = HideFlags.HideAndDontSave;
                }

                DontDestroyOnLoad(obj);
                instance = obj.AddComponent<UnityThread>();
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

#if (ENABLE_UPDATE_FUNCTION_CALLBACK)
        /// <summary>
        /// add action to monobehaviour's corutine
        /// </summary>
        /// <param name="action">action to be performed</param>
        public static void executeCoroutine(IEnumerator action)
        {
            if (instance != null)
            {
                executeInUpdate(() => instance.StartCoroutine(action));
            }
        }

        /// <summary>
        /// add action to unity's main thread
        /// </summary>
        /// <param name="action">action to be performed</param>
        public static void executeInMainThread(Action action)
        {
            executeInUpdate(action);
        }

        /// <summary>
        /// add action to monobehaviour's Update()
        /// </summary>
        /// <param name="action">action to be performed</param>
        private static void executeInUpdate(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            lock (actionQueuesUpdateFunc)
            {
                actionQueuesUpdateFunc.Add(action);
                noActionQueueToExecuteUpdateFunc = false;
            }
        }

        private void Update()
        {
            if (noActionQueueToExecuteUpdateFunc)
            {
                return;
            }

            //Clear the old actions from the actionCopiedQueueUpdateFunc queue
            actionCopiedQueueUpdateFunc.Clear();
            lock (actionQueuesUpdateFunc)
            {
                //Copy actionQueuesUpdateFunc to the actionCopiedQueueUpdateFunc variable
                actionCopiedQueueUpdateFunc.AddRange(actionQueuesUpdateFunc);
                //Now clear the actionQueuesUpdateFunc since we've done copying it
                actionQueuesUpdateFunc.Clear();
                noActionQueueToExecuteUpdateFunc = true;
            }

            // Loop and execute the functions from the actionCopiedQueueUpdateFunc
            for (int i = 0; i < actionCopiedQueueUpdateFunc.Count; i++)
            {
                actionCopiedQueueUpdateFunc[i].Invoke();
            }
        }
#endif

#if (ENABLE_LATEUPDATE_FUNCTION_CALLBACK)
        /// <summary>
        /// add action to monobehaviour's LastUpdate()
        /// </summary>
        /// <param name="action">action to be performed</param>
        public static void executeInLateUpdate(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            lock (actionQueuesLateUpdateFunc)
            {
                actionQueuesLateUpdateFunc.Add(action);
                noActionQueueToExecuteLateUpdateFunc = false;
            }
        }

        private void LateUpdate()
        {
            if (noActionQueueToExecuteLateUpdateFunc)
            {
                return;
            }

            //Clear the old actions from the actionCopiedQueueLateUpdateFunc queue
            actionCopiedQueueLateUpdateFunc.Clear();
            lock (actionQueuesLateUpdateFunc)
            {
                //Copy actionQueuesLateUpdateFunc to the actionCopiedQueueLateUpdateFunc variable
                actionCopiedQueueLateUpdateFunc.AddRange(actionQueuesLateUpdateFunc);
                //Now clear the actionQueuesLateUpdateFunc since we've done copying it
                actionQueuesLateUpdateFunc.Clear();
                noActionQueueToExecuteLateUpdateFunc = true;
            }

            // Loop and execute the functions from the actionCopiedQueueLateUpdateFunc
            for (int i = 0; i < actionCopiedQueueLateUpdateFunc.Count; i++)
            {
                actionCopiedQueueLateUpdateFunc[i].Invoke();
            }
        }
#endif

#if (ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK)
        /// <summary>
        ///  add action to monobehaviour's FixedUpdate()
        /// </summary>
        /// <param name="action">action to be performed</param>
        public static void executeInFixedUpdate(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            lock (actionQueuesFixedUpdateFunc)
            {
                actionQueuesFixedUpdateFunc.Add(action);
                noActionQueueToExecuteFixedUpdateFunc = false;
            }
        }

        private void FixedUpdate()
        {
            if (noActionQueueToExecuteFixedUpdateFunc)
            {
                return;
            }

            //Clear the old actions from the actionCopiedQueueFixedUpdateFunc queue
            actionCopiedQueueFixedUpdateFunc.Clear();
            lock (actionQueuesFixedUpdateFunc)
            {
                //Copy actionQueuesFixedUpdateFunc to the actionCopiedQueueFixedUpdateFunc variable
                actionCopiedQueueFixedUpdateFunc.AddRange(actionQueuesFixedUpdateFunc);
                //Now clear the actionQueuesFixedUpdateFunc since we've done copying it
                actionQueuesFixedUpdateFunc.Clear();
                noActionQueueToExecuteFixedUpdateFunc = true;
            }

            // Loop and execute the functions from the actionCopiedQueueFixedUpdateFunc
            for (int i = 0; i < actionCopiedQueueFixedUpdateFunc.Count; i++)
            {
                actionCopiedQueueFixedUpdateFunc[i].Invoke();
            }
        }
#endif

        private void OnDisable()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}