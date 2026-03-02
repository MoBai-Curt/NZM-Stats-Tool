package com.example.nzmhelper

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.nzmhelper.databinding.ActivityLoginBinding

class LoginActivity : AppCompatActivity() {
    companion object { val apiService = ApiService() }

    private lateinit var binding: ActivityLoginBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnLogin.setOnClickListener {
            val rawCookie = binding.etCookie.text.toString().trim()

            if (rawCookie.isEmpty()) {
                Toast.makeText(this, "Cookie 不能为空", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            val cookieMap = mutableMapOf<String, String>()
            val pairs = rawCookie.split(";")
            for (pair in pairs) {
                val kv = pair.trim().split("=", limit = 2)
                if (kv.size == 2) {
                    cookieMap[kv[0].trim()] = kv[1].trim()
                }
            }

            val acctype = cookieMap["acctype"]
            val openid = cookieMap["openid"]

            when {
                acctype == "qc" && cookieMap.containsKey("access_token") && openid != null -> {
                    val appid = cookieMap["appid"] ?: "1112451898"
                    val accessToken = cookieMap["access_token"]

                    apiService.globalCookie = "openid=$openid; acctype=qc; appid=$appid; access_token=$accessToken;"
                    loginSuccess("QQ 登录")
                }

                acctype == "mini" && cookieMap.containsKey("ieg_ams_token") && openid != null -> {
                    val appid = cookieMap["appid"] ?: "wx4e8cbe4fb0eca54c"
                    val iegToken = cookieMap["ieg_ams_token"]

                    val sessionToken = cookieMap["ieg_ams_session_token"] ?: ""
                    val tokenTime = cookieMap["ieg_ams_token_time"] ?: ""
                    val tokenV2 = cookieMap["ieg_ams_token_v2"] ?: ""

                    val sb = StringBuilder("openid=$openid; acctype=mini; appid=$appid; ieg_ams_token=$iegToken;")
                    if (sessionToken.isNotEmpty()) sb.append(" ieg_ams_session_token=$sessionToken;")
                    if (tokenTime.isNotEmpty()) sb.append(" ieg_ams_token_time=$tokenTime;")
                    if (tokenV2.isNotEmpty()) sb.append(" ieg_ams_token_v2=$tokenV2;")

                    apiService.globalCookie = sb.toString()
                    loginSuccess("微信登录")
                }

                cookieMap.containsKey("uin") && cookieMap.containsKey("skey") -> {
                    val uin = cookieMap["uin"]
                    val skey = cookieMap["skey"]

                    apiService.globalCookie = "uin=$uin; skey=$skey;"
                    loginSuccess("网页 Skey")
                }

                else -> {
                    Toast.makeText(this, "无效的格式！未识别到 QQ、微信或网页关键凭证", Toast.LENGTH_LONG).show()
                }
            }
        }
    }

    private fun loginSuccess(platform: String) {
        Toast.makeText(this, "[$platform] 解析成功，正在进入...", Toast.LENGTH_SHORT).show()
        // 跳转到主页查询战绩
        startActivity(Intent(this, MainActivity::class.java))
        finish()
    }
}