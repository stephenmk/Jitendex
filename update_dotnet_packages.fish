for source_folder in Source/*
    cd $source_folder
    dotnet package update
    cd ../..
end

for source_folder in Libraries/*/Jitendex.*
    cd $source_folder
    dotnet package update
    cd ../../..
end
