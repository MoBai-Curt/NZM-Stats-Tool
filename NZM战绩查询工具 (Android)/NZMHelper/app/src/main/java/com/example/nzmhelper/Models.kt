package com.example.nzmhelper

import com.google.gson.annotations.SerializedName

data class ApiResponse<T>(
    @SerializedName("ret") val ret: Int,
    @SerializedName("jData") val jData: JData<T>?
)

data class JData<T>(
    @SerializedName("data") val data: InnerData<T>?
)

data class InnerData<T>(
    @SerializedName("data") val data: T?
)

data class UserSummary(
    @SerializedName("huntGameCount") val huntGameCount: String?,
    @SerializedName("playtime") val playtime: String?
)

data class GameListResponse(
    @SerializedName("gameList") val gameList: List<GameRecord>?
)

data class GameRecord(
    @SerializedName("iMapId") val mapId: String?,
    @SerializedName("iIsWin") val isWin: String?,
    @SerializedName("iScore") val score: String?,
    @SerializedName("iDuration") val duration: String?,
    @SerializedName("dtGameStartTime") val startTime: String?,
    @SerializedName("DsRoomId") val roomId: String?,
    @SerializedName("iSubModeType") val subMode: String?
)

data class MatchDetail(
    @SerializedName("loginUserDetail") val self: PlayerDetail?,
    @SerializedName("list") val teammates: List<PlayerDetail>?
)

data class PlayerDetail(
    @SerializedName("nickname") val nickname: String?,
    @SerializedName("avatar") val avatar: String?,
    @SerializedName("baseDetail") val base: BaseDetail?,
    @SerializedName("huntingDetails") val hunt: HuntDetail?,
    @SerializedName("equipmentScheme", alternate = ["EquipmentScheme"]) val equipmentScheme: List<EquipmentItem>?
)

data class BaseDetail(
    @SerializedName("iScore") val score: String?,
    @SerializedName("iDeaths") val deaths: String?
)

data class HuntDetail(
    @SerializedName("damageTotalOnBoss") val bossDmg: String?,
    @SerializedName("DamageTotalOnBoss") val bossDmgCap: String?,
    @SerializedName("damageTotalOnMobs") val mobDmg: String?,
    @SerializedName("DamageTotalOnMobs") val mobDmgCap: String?,
    @SerializedName("totalCoin") val coin: String?,
    @SerializedName("TotalCoin") val coinCap: String?,
    @SerializedName("itemProgress") val itemProgress: ItemProgress?,
    @SerializedName("ItemProgress") val itemProgressCap: ItemProgress?,
    @SerializedName("partitionDetails", alternate = ["PartitionDetails"]) val partitionDetails: List<PartitionDetail>?
)

data class PartitionDetail(
    @SerializedName("areaId", alternate = ["AreaId"]) val areaId: String?,
    @SerializedName("usedTime", alternate = ["UsedTime"]) val usedTime: String?
)

data class EquipmentItem(
    @SerializedName("weaponName", alternate = ["WeaponName"]) val weaponName: String?,
    @SerializedName("pic", alternate = ["Pic"]) val pic: String?,
    @SerializedName("quality", alternate = ["Quality"]) val quality: Int?,
    @SerializedName("commonItems", alternate = ["CommonItems"]) val commonItems: List<CommonItem>?
)

data class CommonItem(
    @SerializedName("itemName", alternate = ["ItemName"]) val itemName: String?,
    @SerializedName("pic", alternate = ["Pic"]) val pic: String?
)

data class ItemProgress(
    @SerializedName("current") val current: Int,
    @SerializedName("required") val required: Int,
    @SerializedName("itemName") val itemName: String?
)

data class CollectionResponse(
    @SerializedName("list") val list: List<CollectionItem>?
)

data class FragmentHomeResponse(
    @SerializedName("weaponList") val weaponList: List<CollectionItem>?
)

data class CollectionItem(
    @SerializedName("weaponName") val weaponName: String?,
    @SerializedName("trapName") val trapName: String?,
    @SerializedName("itemName") val itemName: String?,
    @SerializedName("pic") val pic: String?,
    @SerializedName("icon") val icon: String?,
    @SerializedName("quality") val quality: Int?,
    @SerializedName("owned") val owned: Boolean?,
    @SerializedName("itemProgress", alternate = ["ItemProgress"]) val itemProgress: ItemProgress?
)

data class WxTokenResponse(
    val openid: String,
    @SerializedName("access_token") val accessToken: String
)

data class RemoteConfig(
    @SerializedName("LatestVersion") val latestVersion: String?,
    @SerializedName("DownloadUrl") val downloadUrl: String?,
    @SerializedName("UpdatePassword") val updatePassword: String?,
    @SerializedName("Announcement") val announcement: String?
)