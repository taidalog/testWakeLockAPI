module App

open System
open Browser.Dom
open Browser.Navigator
open Fable.Core
open Fable.Core.JsInterop

[<Emit("setTimeout($0, $1)")>]
let setTimeout (code: unit -> unit) (delay: int) : int = jsNative

let mutable wakeLock: JS.Promise<obj> option = None

let lock () =
    promise {
        printfn "locking at %s" (DateTime.Now.ToString())
        let! x = navigator?wakeLock?request("screen")
        return x
    }

let release (x: JS.Promise<obj>) =
    x.``then``(fun x -> 
        printfn "releasing at %s" (DateTime.Now.ToString())
        x?release()
    ) |> ignore

let isWakeLockSupported =
    emitJsStatement
        ()
        """
            "wakeLock" in navigator
        """

let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement
myButton.onclick <- fun _ ->
    myButton.innerText <- sprintf "You clicked on: %s" (DateTime.Now.ToString())

    wakeLock <-
        if isWakeLockSupported then
            try
                lock () |> Some
            with
                | _ -> None
        else
            None
    
    printfn "after locking"
    
    setTimeout
        (fun _ ->
            match wakeLock with
            | Some x -> release x
            | None -> printfn "doing nothing")
        (1000 * 60 * 2)
