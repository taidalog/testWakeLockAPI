module App

open Browser.Dom
open Browser.Types
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
                    //let screenLock
                    //navigator.wakeLock.request('screen').then(x => { screenLock = x })
                    navigator.wakeLock.request('screen').then(x => {
                        console.log(typeof x)
                        console.log(x != null)
                        return x
                    })
                    //let screen = await navigator.wakeLock.request('screen')
                } catch (e) {
                    console.log(e.name, e.message)
                }
                //return screenLock
            } else {
                console.log("no wakeLock API.")
            }
        """

let screenLock2 () =
    emitJsStatement
        ()
        """
            if ('wakeLock' in navigator) {
                try {
                    console.log("with wakeLock API.")
                    //let screenLock
                    //navigator.wakeLock.request('screen').then(x => { screenLock = x })
                    navigator.wakeLock.request('screen').then(x => {
                        console.log(typeof x)
                        console.log(x != null)
                        return (() => {
                            x.release().then(() => {
                                console.log("released wakeLock.")
                            })
                        })
                    })
                    //let screen = await navigator.wakeLock.request('screen')
                } catch (e) {
                    console.log(e.name, e.message)
                }
                //return screenLock
            } else {
                console.log("no wakeLock API.")
            }
        """

let screenLock3 () =
    emitJsStatement
        ()
        """
            if ('wakeLock' in navigator) {
                try {
                    console.log("with wakeLock API.")
                    navigator.wakeLock.request('screen').then(x => {
                        console.log(typeof x)
                        console.log(x != null)
                        state = x
                    })
                } catch (e) {
                    console.log(e.name, e.message)
                }
            } else {
                console.log("no wakeLock API.")
            }
        """

let releaseWakeLock (wakeLock: obj) : unit =
    emitJsStatement
        wakeLock
        """
            console.log(typeof $0)
            console.log($0 != null)
            if (typeof $0 !== "undefined" && $0 != null) {
                $0.release().then(() => {
                    console.log("released wakeLock.")
                })
            } else {
                console.log("failed to release wakeLock.")
            }
        """

let mutable state: obj = null

myButton.onclick <- fun _ ->
    myButton.innerText <- sprintf "You clicked on: %s" (System.DateTime.Now.ToString())
    //let wakeLock = screenLock ()
    //printfn "%s" (wakeLock.GetType().ToString())
    //setTimeout (fun _ -> releaseWakeLock wakeLock) (1000 * 2)

    //let wakeLock2 = navigator?wakeLock?request("screen")
    //printfn "%s" (wakeLock2.GetType().ToString())
    //printfn "%s" (wakeLock2?released)
    //setTimeout (fun _ -> wakeLock2?release()) (1000 * 2) |> ignore
    //setTimeout (fun _ -> releaseWakeLock wakeLock2) (1000 * 2)

    // printfn "%s" (state.GetType().ToString())
    // screenLock3 ()
    // printfn "%s" (state.GetType().ToString())
    // setTimeout (fun _ -> releaseWakeLock state) (1000 * 2)

    let wakeLock3 =
        promise {
            printfn "locking at %s" (System.DateTime.Now.ToString())
            let! x = navigator?wakeLock?request("screen")
            return x
        }
    let wakeLock3' = wakeLock3.``then``(fun x -> x)
    printfn "%A" (wakeLock3'?released)
    let f () =
        wakeLock3.``then``(fun x -> 
            printfn "releasing at %s" (System.DateTime.Now.ToString())
            x?release()
        ) |> ignore
    setTimeout (fun _ -> f ()) (1000 * 60 * 5)
