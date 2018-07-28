using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

class VideoController:MonoBehaviour
{
    private VideoPlayer video;
    private void Awake()
    {
        video = GetComponent<VideoPlayer>();
        video.loopPointReached += LoadMainScene;
    }
    private void LoadMainScene(VideoPlayer vPlayer)
    {
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        //int displayProgress = 0;
        //int toProgress = 0;

       var  async = SceneManager.LoadSceneAsync("01-main");

        async.allowSceneActivation = false;

        //while (async.progress < 0.9f)
        //{
        //    toProgress = (int)async.progress * 100;
        //    while (displayProgress < toProgress)
        //    {
        //        ++displayProgress;

        //        //yield return new WaitForEndOfFrame();
        //    }
        //}
        //toProgress = 100;
        //while (displayProgress < toProgress)
        //{
        //    ++displayProgress;

        //    yield return new WaitForEndOfFrame();
        //}

     
        async.allowSceneActivation = true;

        
        //读取完毕后返回， 系统会自动进入场景
        yield return async;

    }
}
