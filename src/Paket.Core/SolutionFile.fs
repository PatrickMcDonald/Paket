﻿namespace Paket

open System.IO
open Logging
open System

/// Contains methods to read and manipulate solution files.
type SolutionFile(fileName: string) =
    [<Literal>] 
    let slnFolderProjectGuid = "2150E333-8FDC-42A3-9474-1A3956D46DE8"
    let originalContent = File.ReadAllLines fileName |> Array.toList
    let content = ResizeArray( originalContent )
    
    let removeNugetSlnFolderIfEmpty() =
        match content |> Seq.tryFindIndex (fun line -> 
                line.StartsWith(sprintf "Project(\"{%s}\")" slnFolderProjectGuid) && line.Contains(".nuget")) with
        | Some(index) -> 
            if content.[index+1].Contains("ProjectSection(SolutionItems)") &&
               content.[index+2].Contains("EndProjectSection") &&
               content.[index+3].Contains("EndProject")
            then 
                content.RemoveRange(index, 4)
        | None -> ()

    member __.FileName = fileName

    member __.RemoveNugetEntries() =
        for file in ["nuget.targets";"packages.config";"nuget.exe"] do
            match content |> Seq.tryFindIndex (fun line -> line.ToLower().Contains(sprintf ".nuget\\%s" file)) with
            | Some(index) -> content.RemoveAt(index)
            | None -> ()            
        
        removeNugetSlnFolderIfEmpty()

    member __.AddPaketFolder(dependenciesFile, lockFile) =
        
        let lines = ResizeArray<_>()

        lines.Add(sprintf   "Project(\"{%s}\") = \".paket\", \".paket\", \"{%s}\"" slnFolderProjectGuid <| Guid.NewGuid().ToString("D").ToUpper())
        lines.Add           " ProjectSection(SolutionItems) = preProject"
        lines.Add(sprintf   "		%s = %s" dependenciesFile dependenciesFile)

        Option.iter (fun lockFile -> 
            lines.Add(sprintf"		%s = %s" lockFile lockFile)) lockFile

        lines.Add           "	EndProjectSection"
        lines.Add           "EndProject"

        match content |> Seq.tryFindIndex (fun line -> line.StartsWith("Project")) with
        | Some index -> 
            // insert before the first project in a solution
            content.InsertRange(index, lines)
        | None -> 
            // there are no project in solution
            match content |> Seq.tryFindIndex (fun line -> line.StartsWith("Global")) with
            | Some index -> 
                // insert before ``global`` entry
                content.InsertRange(index, lines)
            | None -> 
                // cannot find ``global`` entry, just append to the end of file
                content.InsertRange(content.Count, lines)

    member __.Save() =
        if content |> Seq.toList <> originalContent 
        then 
            File.WriteAllLines(fileName, content)
            tracefn "Solution %s changed" fileName