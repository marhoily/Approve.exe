# Release instructions

1) Go to the assemblyInfo.cs and change the version there
2) Go to the ApprovalsUi.nuspec and chage the version there too
3) Change configuration to Release
4) Build all
5) Go to the package manager console and execute*:
  
  nuget pack .\ApprovalsUi.nuspec

6) Then execute:

  squirrel --releasify .\ApprovalsUi.<new version here!>.nupkg

7) Commit release folder to the github. Don't forget to include "delta" and "full" packages

8) Remove the nupkg file from the root folder

-----
* - if you don't have nuget installed, run:

  choco install nuget.commandline