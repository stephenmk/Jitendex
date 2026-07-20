fmt:
  dotnet format --verbosity normal

jmdict:
  dotnet run -c Release --project Source/Import.JMdict

edrdg-update:
  git -C Data/edrdg-dictionary-archive pull origin main
  git -C Data/edrdg-dictionary-archive checkout main
  git -C Data/edrdg-dictionary-archive verify-commit HEAD

  dotnet run -c Release --project Source/Import.JMdict
  dotnet run -c Release --project Source/Import.JMnedict
  dotnet run -c Release --project Source/Import.Kanjidic2
  dotnet run -c Release --project Source/Import.Tatoeba
  dotnet run -c Release --project Source/Forks.JMdict
  dotnet run -c Release --project Source/Forks.Tatoeba

home-import:
  dotnet run -c Release --project Source/Import.Home import

home-export:
  dotnet run -c Release --project Source/Import.Home export

home-cycle:
  @just home-import
  @just home-export

alias e := edrdg-update
