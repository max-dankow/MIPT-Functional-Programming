module lab1

open System
open System.Net
open System.IO
open System.Collections.Specialized

// почтовый адрес
let email = "max.dankow@gmail.com"
// общий тип для возвращаемых вашими функциями значений, где первая часть кортежа это само значение функции, вторая - кол-во операций
type Result = float * int
let delta = 1e-10

// *** Первая часть

let fTailor x : float = 3. ** x // функция, которую раскладываем f(x) = 3^x
let n, a, b = 20., 0., 1. // интервал [0; 1]

// Раскладывает функцию в ряд Тейлора, в лоб считая члены.
let private nthMember (x:float) k = 
    let fac = function 
        | 0 | 1 -> 1.
        | n -> [for k in [1..n] -> float k] |> List.reduce ( * )
    ((log 3. * x) ** float k) / float (fac k)

let rec private tailor_ acc x : Result =
    if abs(fst acc - fTailor x) < delta
    then acc
    else let k = snd acc in tailor_ (fst acc + nthMember x k,  k + 1) x

let tailor = tailor_ (0., 0)

// Раскладывает функцию в ряд Тейлора домножением на предыдущий член.
let private pow3_ x k (prev, sum) =
    let current = prev *  log 3. * x / (float k) in
    (current, sum + current)

let rec private tailorA_ x fnext acc = 
    let (prev, sum, k) = acc
    if abs(sum - fTailor x) < delta
    then acc
    else 
        let (cur, cursum) = fnext x k (prev, sum)
        tailorA_ x (fnext) (cur, cursum, k + 1)

let tailorA x : Result = let (prev, sum, k) = tailorA_ x (pow3_) (1., 1., 1) in (sum, k)

let printTailor () = 
    [a .. (b-a)/n .. b] 
    |> List.map (fun x -> let (firstRes, firstCou), (secondRes, secondCou) = tailor x, tailorA x in (x, firstRes, firstCou, secondRes, secondCou, fTailor x))
    |> List.iter (fun (a,b,c,d,e,f) -> printf "%f\t%f\t%d\t%f\t%d\t%f\n" a b c d e f )

printTailor()
// *** Вторая часть

let fSolve = fun x -> x // функция, решение которой ищем

let iter f a b : Result = (42., 0)
let newton f a b : Result = (42., 0)
let dichotomy =
    // для функций с аккумулятором удобно ставить его в начало
    let rec dichotomyA i (f:float->float) (a:float) (b:float) : Result = (42., 0) 
    dichotomyA 0 // чтобы воспользоваться каррированием

let printSolve () =
    [iter; newton; dichotomy] 
    |> List.map (fun f -> f fSolve a b) 
    |> List.iter (fun (res, cou) -> printf "%f\t%d\n" res cou)

(*let main () = 
  let values = new NameValueCollection()
  values.Add("email", email)
  values.Add("content", File.ReadAllText(__SOURCE_DIRECTORY__ + @"/" + __SOURCE_FILE__))

  let client = new WebClient()
  let response = client.UploadValues(new Uri("http://mipt.eu01.aws.af.cm/lab1"), values)
  let responseString = Text.Encoding.Default.GetString(response)

  printf "%A\n" responseString*)
