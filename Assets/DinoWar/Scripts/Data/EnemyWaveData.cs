using System;

[Serializable]
public class EnemyWaveData
{
    public enum WaveType
    {
        WaveType_Normal = 0,
        WaveType_Melee,
        WaveType_Shooter,
        WaveType_Speedy
    }

    public WaveType waveType;
    public float spawningPower;
    public int waveDuration;
    public int genMeleeTypeRate;
    public float meleeTypeCost = 1;
    public int genShortRangeTypeRate;
    public float shortRangeTypeCost = 1;
    public int genLongRangeTypeRate;
    public float longRangeTypeCost = 1;
    public int genSpecialTypeRate;
    public float specialTypeCost = 1;
    public int genSupportTypeRate;
    public float supportTypeCost = 1;

    public static EnemyWaveData GetWaveByType(WaveType type) {
        switch(type) {
            case WaveType.WaveType_Normal:
                return EnemyWaveData.GetNormalWaveData();

            case WaveType.WaveType_Melee:
                return EnemyWaveData.GetMeleeWaveData();

            case WaveType.WaveType_Shooter:
                return EnemyWaveData.GetShootersWaveData();

            case WaveType.WaveType_Speedy:
                return EnemyWaveData.GetSpeedyWaveData();
        }

        return null;
    }

    public static EnemyWaveData GetNormalWaveData() {
        EnemyWaveData data = new EnemyWaveData();
        data.waveType = WaveType.WaveType_Normal;
        data.spawningPower = 18;
        data.waveDuration = 300;
        data.genMeleeTypeRate = 20;
        data.meleeTypeCost = 1;
        data.genShortRangeTypeRate = 20;
        data.shortRangeTypeCost = 1;
        data.genLongRangeTypeRate = 20;
        data.longRangeTypeCost = 1;
        data.genSpecialTypeRate = 20;
        data.specialTypeCost = 1;
        data.genSupportTypeRate = 20;
        data.supportTypeCost = 1;

        return data;
    }

    public static EnemyWaveData GetShootersWaveData() {
        EnemyWaveData data = new EnemyWaveData();
        data.waveType = WaveType.WaveType_Shooter;
        data.spawningPower = 18;
        data.waveDuration = 300;
        data.genMeleeTypeRate = 0;
        data.genShortRangeTypeRate = 40;
        data.shortRangeTypeCost = 1;
        data.genLongRangeTypeRate = 60;
        data.longRangeTypeCost = 0.7f;
        data.genSpecialTypeRate = 0;
        data.genSupportTypeRate = 0;

        return data;
    }

    public static EnemyWaveData GetMeleeWaveData() {
        EnemyWaveData data = new EnemyWaveData();
        data.waveType = WaveType.WaveType_Melee;
        data.spawningPower = 18;
        data.waveDuration = 300;
        data.genMeleeTypeRate = 60;
        data.meleeTypeCost = 0.7f;
        data.genShortRangeTypeRate = 0;
        data.genLongRangeTypeRate = 0;
        data.genSpecialTypeRate = 20;
        data.specialTypeCost = 0.7f;
        data.genSupportTypeRate = 20;
        data.supportTypeCost = 1;

        return data;
    }

    public static EnemyWaveData GetSpeedyWaveData() {
        EnemyWaveData data = new EnemyWaveData();
        data.waveType = WaveType.WaveType_Speedy;
        data.spawningPower = 9;
        data.waveDuration = 120;
        data.genMeleeTypeRate = 33;
        data.meleeTypeCost = 1;
        data.genShortRangeTypeRate = 33;
        data.shortRangeTypeCost = 1;
        data.genLongRangeTypeRate = 33;
        data.longRangeTypeCost = 1;
        data.genSpecialTypeRate = 1;
        data.specialTypeCost = 0.2f;
        data.genSupportTypeRate = 0;

        return data;
    }

}
