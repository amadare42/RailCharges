using RailCharges.Configuration;

namespace RailCharges.Patches;

public interface IPatch
{
    void Patch(PluginConfig config);
}