﻿/// Contains methods for addition of new packages
module Paket.AddProcess

open Paket
open System.IO

let Add(package, version, force, hard, interactive, installAfter) =
    let dependenciesFile =
        DependenciesFile.ReadFromFile(Constants.DependenciesFile)
          .Add(package,version)

    let resolution = dependenciesFile.Resolve(force)
    let resolvedPackages = resolution.ResolvedPackages.GetModelOrFail()

    if interactive then
        let di = DirectoryInfo(".")
        for project in ProjectFile.FindAllProjects(".") do
            if Utils.askYesNo(sprintf "  Install to %s?" (project.FileName.Replace(di.FullName,""))) then
                let proj = FileInfo(project.FileName)
                match ProjectFile.FindReferencesFile proj with
                | None ->
                    let newFileName =
                        let fi = FileInfo(Path.Combine(proj.Directory.FullName,Constants.ReferencesFile))
                        if fi.Exists then
                            Path.Combine(proj.Directory.FullName,proj.Name + "." + Constants.ReferencesFile)
                        else
                            fi.FullName

                    File.WriteAllLines(newFileName,[package])
                | Some fileName -> File.AppendAllLines(fileName,["";package])

    if installAfter then
        let lockFileName = DependenciesFile.FindLockfile Constants.DependenciesFile
    
        let lockFile =                
            let lockFile = LockFile(lockFileName.FullName, dependenciesFile.Options, resolvedPackages, resolution.ResolvedSourceFiles)
            lockFile.Save()
            lockFile

        let sources =
            Constants.DependenciesFile
            |> File.ReadAllLines
            |> PackageSourceParser.getSources 

        InstallProcess.Install(sources, force, hard, lockFile)

    dependenciesFile.Save()