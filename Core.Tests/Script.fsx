// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// Define your library scripting code here

type ParserResult<'a> =
    | Success of 'a * list<char>
    | Failure

type Parser<'a> = list<char> -> ParserResult<'a>


let Return (x: 'a): Parser<'a> =
    let p stream = Success(x, stream)
    in p

let Bind (p: Parser<'a>) (f: 'a -> Parser<'b>) : Parser<'b> =
    let q stream =
        match p stream with
        | Success(x, rest) -> (f x) rest
        | Failure -> Failure
    in q


type ParserBuilder() =
    member x.Bind(p, f) = Bind p f
    member x.Return(y) = Return y

let parse = new ParserBuilder()

let single p =
    parse {
        let! x = p
        return x
    } 

let rec Many p : Parser<list<'a>> =
    parse {
        let! x = p          // Applies p
        let! xs = (Many p)  // Applies (Many p) recursively
        return x :: xs      // returns the cons of the two results
    } 
