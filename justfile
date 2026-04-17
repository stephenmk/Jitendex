fmt:
  dotnet format --verbosity normal

jmdict:
  dotnet run -c Release --project Source/Import.JMdict

edrdg-update:
  dotnet run -c Release --project Source/Import.JMdict
  dotnet run -c Release --project Source/Import.JMnedict
  dotnet run -c Release --project Source/Import.Kanjidic2
  dotnet run -c Release --project Source/Import.Tatoeba
  dotnet run -c Release --project Source/Forks.JMdict
  dotnet run -c Release --project Source/Forks.Tatoeba

alias e := edrdg-update
