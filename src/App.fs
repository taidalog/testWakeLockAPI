module App

open Browser.Dom
open Browser.Navigator
open Fable.Core
open Fable.Core.JsInterop

let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement

[<Emit("setTimeout($0, $1)")>]
let setTimeout (code: unit -> unit) (delay: int) : int = jsNative

[<Emit("setInterval($0, $1)")>]
let setInterval (callback: unit -> unit) (interval: int) : int = jsNative

[<Emit("clearInterval($0)")>]
let clearInterval (intervalID: int) : unit = jsNative

let screenLock () =
    emitJsStatement
        ()
        """
            if ('wakeLock' in navigator) {
                try {
                    console.log("with wakeLock API.")
                    let screenLock
                    navigator.wakeLock.request('screen').then(x => { screenLock = x })
                    // let screen = navigator.wakeLock.request('screen')
                } catch (e) {
                    console.log(e.name, e.message)
                }
                return screenLock
            } else {
                console.log("no wakeLock API.")
            }
        """

let releaseWakeLock wakeLock =
    emitJsStatement
        wakeLock
        """
            wakeLock.release()
        """

myButton.onclick <- fun _ ->
    myButton.innerText <- sprintf "You clicked on: %s" (System.DateTime.Now.ToString())
    let wakeLock = screenLock ()
    setTimeout (fun _ -> releaseWakeLock wakeLock) (1000 * 120)
