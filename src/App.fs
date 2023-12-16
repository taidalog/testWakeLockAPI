module App

open System
open Browser.Dom
open Browser.Types
open Browser.Navigator
open Fable.Core
open Fable.Core.JsInterop

let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement

[<Emit("setTimeout($0, $1)")>]
let setTimeout (code: unit -> unit) (delay: int) : int = jsNative

let mutable state: obj = null

myButton.onclick <- fun _ ->
    myButton.innerText <- sprintf "You clicked on: %s" (DateTime.Now.ToString())

    let wakeLock3 =
        promise {
            printfn "locking at %s" (DateTime.Now.ToString())
            let! x = navigator?wakeLock?request("screen")
            return x
        }
    
    printfn "after locking"

    setTimeout
        (fun _ ->
            wakeLock3.``then``(fun x -> 
                printfn "releasing at %s" (DateTime.Now.ToString())
                x?release()
            ) |> ignore)
        (1000 * 60 * 2)
