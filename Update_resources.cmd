echo "Update the git submodules."
git submodule update --init --recursive --remote

echo "Update common string resource classes via the resources generator."

.\ResourcesGenerator\src\ResourceGenerator\bin\Debug\net6.0\ResourceGenerator.exe dotnet .\ResourcesGenerator\CommonResources .\MlemApi\Resources
