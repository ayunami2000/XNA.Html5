using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H5;
using static H5.Core.dom;

namespace Microsoft.Xna.Framework
{
    public class WebAudioHelper
    {
        public static void Init()
        {
            var script = new HTMLScriptElement();
            script.innerHTML = @"function BufferLoader(context, urlList, callback) {
    this.context = context;
    this.urlList = urlList;
    this.onload = callback;
    this.bufferList = new Array();
    this.loadCount = 0;
}

BufferLoader.prototype.loadBuffer = function (url, index) {
    // Load buffer asynchronously
    var request = new XMLHttpRequest();
    request.open('GET', url, true);
    request.responseType = 'arraybuffer';

    var loader = this;

    request.onload = function () {
        // Asynchronously decode the audio file data in request.response
        loader.context.decodeAudioData(
            request.response,
            function (buffer) {
                if (!buffer) {
                    alert('error decoding file data: ' + url);
                    return;
                }
                loader.bufferList[index] = buffer;
                if (++loader.loadCount == loader.urlList.length)
                    loader.onload(loader.bufferList);
            },
            function (error) {
                console.error('decodeAudioData error', error);
            }
        );
    }

    request.onerror = function () {
        alert('BufferLoader: XHR error');
    }

    request.send();
}

BufferLoader.prototype.load = function () {
    for (var i = 0; i < this.urlList.length; ++i)
        this.loadBuffer(this.urlList[i], i);
}

function AudioContextManager() {
    this.context = null;
    this.bufferLoader = null;
    this.bufferList = null;
    this.loop = false;
}

AudioContextManager.prototype.load = function (src, callback) {
    window.AudioContext = window.AudioContext || window.webkitAudioContext;
    if (window.AudioContext) {
        this.context = new AudioContext();
        var that = this;
        bufferLoader = new BufferLoader(
            this.context,
            [
                src
            ],
            finishedLoading = function (list) {
                that.bufferList = list;
                callback();
            }
        );

        bufferLoader.load();
    } else {
        callback();
    }
}

AudioContextManager.prototype.play = function () {
    if (window.AudioContext) {
        var source = this.context.createBufferSource();
        source.buffer = this.bufferList[0];
        source.loop = this.loop;
        source.connect(this.context.destination);
        source.start(0);
    }
}

var audioContexts = [];

function playAllAudios() {
    for (var i = 0; i < audioContexts.length; i++) {
        audioContexts[i].play();
    }
}

function addAudioContext() {
    audioContexts.push(new AudioContextManager());
    return audioContexts.length - 1;
}

function removeAllAudioContexts() {
    audioContexts = [];
}";
            document.body.appendChild(script);
        }

        public static void Activate()
        {
            Script.Write("playAllAudios();");
        }
    }
}
