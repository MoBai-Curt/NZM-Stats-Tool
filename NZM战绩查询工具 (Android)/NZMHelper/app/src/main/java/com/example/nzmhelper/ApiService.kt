package com.example.nzmhelper

import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okhttp3.*
import java.util.concurrent.TimeUnit
import java.util.regex.Pattern

class ApiService {
    private val WX_UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 MicroMessenger/7.0.20.1781(0x6700143B) NetType/WIFI MiniProgramEnv/Android"
    private val QQ_UA = "Mozilla/5.0 (Linux; Android 12; Pixel 6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Mobile Safari/537.36"

    private val gson = Gson()
    var globalCookie: String = ""
    private var qrsig: String = ""

    private val client: OkHttpClient = OkHttpClient.Builder()
        .connectTimeout(15, TimeUnit.SECONDS)
        .addInterceptor { chain ->
            val original = chain.request()
            val url = original.url.toString()
            val builder = original.newBuilder()
            if (url.contains("comm.ams.game.qq.com")) {
                val isWx = globalCookie.contains("openid=")
                if (isWx) {
                    builder.header("User-Agent", WX_UA)
                    builder.header("Referer", "https://servicewechat.com/wx4e8cbe4fb0eca54c/9/page-frame.html")
                } else {
                    builder.header("User-Agent", QQ_UA)
                    builder.header("Referer", "https://nzm.qq.com/")
                }
                builder.header("xweb_xhr", "1")
                if (globalCookie.isNotEmpty()) builder.header("Cookie", globalCookie.trim())
            } else {
                builder.header("User-Agent", QQ_UA)
            }
            chain.proceed(builder.build())
        }
        .build()

    companion object {
        const val CURRENT_VERSION = "V1.2.0"
        const val SEASON_ID = 1
        val MAP_CONFIG: Map<String, Triple<String, String, String>> = mapOf(
            "1000" to Triple("风暴峡谷", "机甲战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1000.png"),
            "1001" to Triple("风暴峡谷", "机甲战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1001.png"),
            "1002" to Triple("凯旋之地", "机甲战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1002.png"),
            "112" to Triple("黑暗复活节", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-112.png"),
            "114" to Triple("大都会", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-114.png"),
            "115" to Triple("冰点源起", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-115.png"),
            "12" to Triple("黑暗复活节", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-12.png"),
            "132" to Triple("飓风要塞", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-132.png"),
            "135" to Triple("苍穹之上", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-135.png"),
            "14" to Triple("大都会", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-14.png"),
            "16" to Triple("昆仑神宫", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-16.png"),
            "17" to Triple("精绝古城", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-17.png"),
            "21" to Triple("冰点源起", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-21.png"),
            "30" to Triple("猎场-新手关", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-30.png"),
            "300" to Triple("空间站", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-300.png"),
            "304" to Triple("20号星港", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-304.png"),
            "306" to Triple("联盟大厦", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-306.png"),
            "308" to Triple("塔防-新手关", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-308.png"),
            "321" to Triple("根除变异", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-321.png"),
            "322" to Triple("夺回资料", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-322.png"),
            "323" to Triple("猎杀南十字", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-323.png"),
            "324" to Triple("追猎-新手关", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-324.png")
        )

        fun getModeName(mapId: String?): String = MAP_CONFIG[mapId]?.second ?: "未知模式"

        fun getSubModeName(subMode: String?): String {
            return when (subMode) {
                "0" -> "默认"
                "1" -> "引导"
                "2" -> "普通"
                "3" -> "困难"
                "4" -> "英雄"
                "5" -> "炼狱"
                "6" -> "折磨I"
                "7" -> "折磨II"
                "8" -> "折磨III"
                "9" -> "折磨IV"
                "10" -> "折磨V"
                "11" -> "折磨VI"
                "32" -> "练习"
                "64" -> "最大值"
                else -> ""
            }
        }
    }

    fun clearCookie() {
        globalCookie = ""
        qrsig = ""
    }

    suspend fun getQqQrCode(): ByteArray? = withContext(Dispatchers.IO) {
        val url = "https://ssl.ptlogin2.qq.com/ptqrshow?appid=549000912&e=2&l=M&s=3&d=72&v=4&t=${Math.random()}&daid=5"
        try {
            val response = client.newCall(Request.Builder().url(url).build()).execute()
            val cookie = response.headers("Set-Cookie").find { it.contains("qrsig") }
            if (cookie != null) qrsig = cookie.substringAfter("qrsig=").substringBefore(";")
            response.body?.bytes()
        } catch (e: Exception) { null }
    }

    suspend fun checkQqQr(): Int = withContext(Dispatchers.IO) {
        if (qrsig.isEmpty()) return@withContext -1
        var hash = 0
        for (i in qrsig.indices) hash += (hash shl 5) + qrsig[i].code
        val token = hash and 0x7fffffff
        val url = "https://ssl.ptlogin2.qq.com/ptqrlogin?u1=https%3A%2F%2Fqzone.qq.com%2F&ptqrtoken=$token&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=0-0-${System.currentTimeMillis()}&js_ver=21020514&js_type=1&login_sig=&pt_uistyle=40&aid=549000912&daid=5"
        try {
            val response = client.newCall(Request.Builder().url(url).header("Cookie", "qrsig=$qrsig").header("Referer", "https://qzone.qq.com/").build()).execute()
            val text = response.body?.string() ?: ""
            if (text.contains("ptuiCB('0'")) {
                val headers = response.headers("Set-Cookie")
                var uin = ""
                var skey = ""
                headers.forEach {
                    if (it.contains("uin=")) uin = it.substringAfter("uin=").substringBefore(";")
                    if (it.contains("skey=")) skey = it.substringAfter("skey=").substringBefore(";")
                }
                globalCookie = "uin=$uin; skey=$skey;"
                0
            } else {
                if (text.contains("'66'")) 66 else 65
            }
        } catch (e: Exception) { -1 }
    }

    suspend fun getWxQrCode(): Pair<ByteArray?, String?> = withContext(Dispatchers.IO) {
        val url = "https://open.weixin.qq.com/connect/qrconnect?appid=wxfa0c35392d06b82f&redirect_uri=https%3A%2F%2Fiu.qq.com%2Fcomm-htdocs%2Flogin%2Fmilosdk%2Fwx_pc_redirect.html&response_type=code&scope=snsapi_login&state=1"
        try {
            val response = client.newCall(Request.Builder().url(url).build()).execute()
            val html = response.body?.string() ?: ""
            val m = Pattern.compile("/connect/qrcode/([a-zA-Z0-9_-]+)").matcher(html)
            if (m.find()) {
                val uuid = m.group(1) ?: return@withContext Pair(null, null)
                val imgRes = client.newCall(Request.Builder().url("https://open.weixin.qq.com/connect/qrcode/$uuid").build()).execute()
                Pair(imgRes.body?.bytes(), uuid)
            } else Pair(null, null)
        } catch (e: Exception) { Pair(null, null) }
    }

    suspend fun checkWxQr(uuid: String): Int = withContext(Dispatchers.IO) {
        val url = "https://long.open.weixin.qq.com/connect/l/qrconnect?uuid=$uuid&_=${System.currentTimeMillis()}"
        try {
            val response = client.newCall(Request.Builder().url(url).build()).execute()
            val text = response.body?.string() ?: ""
            val err = Pattern.compile("wx_errcode=(\\d+)").matcher(text)
            if (err.find()) {
                val code = err.group(1)?.toInt() ?: -1
                if (code == 405) {
                    val wx = Pattern.compile("wx_code='([^']*)'").matcher(text)
                    if (wx.find() && exchangeWxToken(wx.group(1))) 0 else -1
                } else code
            } else -1
        } catch (e: Exception) { 408 }
    }

    private suspend fun exchangeWxToken(code: String?): Boolean = withContext(Dispatchers.IO) {
        val url = "https://apps.game.qq.com/ams/ame/codeToOpenId.php?appid=wxfa0c35392d06b82f&wxcode=$code&acctype=wx&sServiceType=undefined&wxcodedomain=nzm.qq.com&callback=_cb"
        try {
            val response = client.newCall(Request.Builder().url(url).header("Referer", "https://nzm.qq.com/").build()).execute()
            val text = response.body?.string() ?: ""
            val m = Pattern.compile("_cb\\((.*)\\)").matcher(text)
            if (m.find()) {
                val outer = gson.fromJson(m.group(1), com.google.gson.JsonObject::class.java)
                if (outer.get("iRet").asInt == 0) {
                    val data = gson.fromJson(outer.get("sMsg").asString, WxTokenResponse::class.java)
                    globalCookie = "acctype=wx; openid=${data.openid}; appid=wxfa0c35392d06b82f; access_token=${data.accessToken};"
                    return@withContext true
                }
            }
            false
        } catch (e: Exception) { false }
    }

    suspend fun fetchRemoteConfig(): RemoteConfig? = withContext(Dispatchers.IO) {
        try {
            val response = client.newCall(Request.Builder().url("https://mobaiya.icu/files/android.json").build()).execute() // 用于更新为个人服务器不上传和下载任何东西
            gson.fromJson(response.body?.string(), RemoteConfig::class.java)
        } catch (e: Exception) { null }
    }

    suspend fun getSummary(): UserSummary? = postApi("center.user.stats", mapOf("seasonID" to SEASON_ID), UserSummary::class.java)
    suspend fun getHistory(page: Int): List<GameRecord>? = postApi("center.user.game.list", mapOf("seasonID" to SEASON_ID, "page" to page, "limit" to 15), GameListResponse::class.java)?.gameList
    suspend fun getMatchDetail(roomId: String): MatchDetail? = postApi("center.game.detail", mapOf("seasonID" to 1, "roomID" to roomId), MatchDetail::class.java)
    suspend fun getWeaponCollection(): List<CollectionItem>? = postApi("collection.weapon.list", mapOf("seasonID" to SEASON_ID, "queryTime" to true), CollectionResponse::class.java)?.list
    suspend fun getTrapCollection(): List<CollectionItem>? = postApi("collection.trap.list", mapOf("seasonID" to SEASON_ID), CollectionResponse::class.java)?.list
    suspend fun getPluginCollection(): List<CollectionItem>? = postApi("collection.plugin.list", mapOf("seasonID" to SEASON_ID), CollectionResponse::class.java)?.list

    suspend fun getFragmentHome(): List<CollectionItem>? {
        return postApi(
            "collection.home",
            mapOf("seasonID" to SEASON_ID, "limit" to 100),
            FragmentHomeResponse::class.java,
            "http://wechatmini.qq.com/-/-/pages/index/index/"
        )?.weaponList
    }

    private suspend fun <T> postApi(
        method: String,
        param: Any,
        clazz: Class<T>,
        customEasUrl: String? = null
    ): T? = withContext(Dispatchers.IO) {
        val body = FormBody.Builder()
            .add("iChartId", "430662")
            .add("iSubChartId", "430662")
            .add("sIdeToken", "NoOapI")
            .add("method", method)
            .add("from_source", "2")
            .add("param", gson.toJson(param))
        if (customEasUrl != null) {
            body.add("eas_url", customEasUrl)
        } else if (globalCookie.contains("openid=")) {
            body.add("eas_url", "http://wechatmini.qq.com/-/-/pages/record/record/")
        }

        try {
            val request = Request.Builder().url("https://comm.ams.game.qq.com/ide/").post(body.build()).build()
            val response = client.newCall(request).execute()
            val json = response.body?.string() ?: ""
            val type = TypeToken.getParameterized(ApiResponse::class.java, clazz).type
            val res: ApiResponse<T> = gson.fromJson(json, type)
            res.jData?.data?.data
        } catch (e: Exception) { null }
    }
}