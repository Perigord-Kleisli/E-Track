{
  description = "A very basic flake";

  inputs = {
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix/main";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = {
    self,
    nuget-packageslock2nix,
    nixpkgs,
  }: let
    system = "x86_64-linux";
    pkgs = import nixpkgs {inherit system;};
  in {
    packages.x86_64-linux.e-tracker = pkgs.buildDotnetModule {
      pname = "e-tracker";
      version = "0.1.0";

      dotnet-sdk = pkgs.dotnetCorePackages.sdk_7_0;
      dotnet-runtime = pkgs.dotnetCorePackages.runtime_7_0;

      src = ./.;
      projectFile = "e-track.csproj";
      nugetDeps = nuget-packageslock2nix.lib {
        system = "x86_64-linux";
        lockfiles = [./packages.lock.json];
      };

      meta = with pkgs.lib; {
        license = licenses.mit;
      };
    };

    packages.x86_64-linux.default = self.packages.x86_64-linux.e-tracker;

    devShells.x86_64-linux.default = pkgs.mkShell {
      buildInputs = [
        pkgs.dotnetCorePackages.sdk_7_0
        pkgs.dotnetCorePackages.runtime_7_0
        pkgs.omnisharp-roslyn
        pkgs.nginx
      ];
    };
  };
}
