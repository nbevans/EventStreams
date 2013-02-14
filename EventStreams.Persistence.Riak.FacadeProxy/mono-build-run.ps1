if ($env:Path -notlike  "*\Mono*") {
  $env:Path += ";" + (join-path ${Env:ProgramFiles(x86)} "Mono-2.10.9\bin")
}

echo "Building with XBuild..."
xbuild /p:TargetFrameworkProfile="" /p:Configuration=Debug /p:Platform=AnyCPU

echo "Executing with Mono Runtime..."
mono --gc=sgen "bin\Debug\Horizon.exe"

