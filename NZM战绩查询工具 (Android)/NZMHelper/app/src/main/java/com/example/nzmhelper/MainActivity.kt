package com.example.nzmhelper

import android.content.Intent
import android.graphics.Typeface
import android.net.Uri
import android.os.Bundle
import android.view.View
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.nzmhelper.databinding.ActivityMainBinding
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import java.text.SimpleDateFormat
import java.util.Locale

class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding
    private val api = LoginActivity.apiService
    private val allMatches = ArrayList<GameRecord>()
    private var currentType = 0
    private var currentQualityFilter = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        checkRemoteConfig()

        binding.recyclerHistory.layoutManager = LinearLayoutManager(this)
        binding.recyclerMapStats.layoutManager = LinearLayoutManager(this)
        binding.viewCollection.layoutManager = GridLayoutManager(this, 3)

        binding.bottomNav.findViewById<View>(R.id.btnNavStats).setOnClickListener { switchNav(true) }
        binding.bottomNav.findViewById<View>(R.id.btnNavCollection).setOnClickListener {
            switchNav(false)
            loadCollectionData(currentType)
        }

        binding.tabWeapon.setOnClickListener { currentQualityFilter = 0; loadCollectionData(0) }
        binding.tabTrap.setOnClickListener { currentQualityFilter = 0; loadCollectionData(1) }
        binding.tabPlugin.setOnClickListener { currentQualityFilter = 0; loadCollectionData(2) }

        binding.tabFragment.setOnClickListener { currentQualityFilter = 0; loadCollectionData(3) }

        binding.tabWeapon.setOnLongClickListener { showQualityDialog(0); true }
        binding.tabPlugin.setOnLongClickListener { showQualityDialog(2); true }

        binding.header.findViewById<View>(R.id.tvLogout).setOnClickListener {
            AlertDialog.Builder(this).setTitle("提示").setMessage("确定要退出吗？")
                .setPositiveButton("退出") { _, _ ->
                    api.clearCookie()
                    startActivity(Intent(this, LoginActivity::class.java))
                    finish()
                }.setNegativeButton("取消", null).show()
        }

        loadAllMatches()
    }

    private fun checkRemoteConfig() {
        lifecycleScope.launch {
            val config = api.fetchRemoteConfig() ?: return@launch
            if (!config.announcement.isNullOrEmpty()) {
                AlertDialog.Builder(this@MainActivity).setTitle("系统公告").setMessage(config.announcement).setPositiveButton("知道了", null).show()
            }
            if (config.latestVersion != null && config.latestVersion != ApiService.CURRENT_VERSION) {
                AlertDialog.Builder(this@MainActivity).setTitle("发现新版本").setMessage("前往下载新版本？")
                    .setPositiveButton("去下载") { _, _ -> startActivity(Intent(Intent.ACTION_VIEW, Uri.parse(config.downloadUrl ?: "http://mobaiya.icu"))) }.show()
            }
        }
    }

    private fun showQualityDialog(type: Int) {
        val options = arrayOf("全部", "传说", "史诗", "稀有")
        AlertDialog.Builder(this).setTitle("选择分类").setItems(options) { _, which ->
            currentQualityFilter = when (which) { 1 -> 4; 2 -> 3; 3 -> 2; else -> 0 }
            val label = if (currentQualityFilter == 0) "" else options[which] + "-"
            if (type == 0) binding.tabWeapon.text = "${label}武器"
            if (type == 2) binding.tabPlugin.text = "${label}插件"
            loadCollectionData(type)
        }.show()
    }

    private fun loadAllMatches() {
        binding.pbLoading.visibility = View.VISIBLE
        binding.tvLoadStatus.visibility = View.VISIBLE
        lifecycleScope.launch {
            val summary = api.getSummary()
            if (summary != null) {
                binding.tvTotalGames.text = summary.huntGameCount ?: "0"
                binding.tvTotalTime.text = getString(R.string.play_time, (summary.playtime?.toIntOrNull() ?: 0) / 60)
            }
            allMatches.clear()
            var page = 1
            while (isActive) {
                val list = api.getHistory(page)
                if (list.isNullOrEmpty()) break
                allMatches.addAll(list.filter { (it.mapId?.toIntOrNull() ?: 0) < 1000 })
                page++
            }
            binding.pbLoading.visibility = View.GONE
            binding.tvLoadStatus.visibility = View.GONE
            if (allMatches.isNotEmpty()) {
                binding.recyclerHistory.adapter = GameAdapter(allMatches)
                updateGlobalStats()
                generateMapStats()
            }
        }
    }

    private fun updateGlobalStats() {
        val total = allMatches.size
        val wins = allMatches.count { it.isWin == "1" }
        binding.tvRecentCount.text = "统计场次: $total"; binding.tvWinRate.text = "胜率: ${(wins * 100.0 / total).toInt()}%"
        val sdf = SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault())
        val thirtyDaysAgo = System.currentTimeMillis() - 30L * 24 * 60 * 60 * 1000
        val recentMatches = allMatches.filter { (it.startTime?.let { t -> sdf.parse(t)?.time } ?: 0L) >= thirtyDaysAgo }
        if (recentMatches.isNotEmpty()) {
            val rWins = recentMatches.count { it.isWin == "1" }
            binding.tvRecentPeriod.text = "近期统计 (近${recentMatches.size}场)"
            binding.tvRecentWinRate.text = "近期胜率: ${(rWins * 100.0 / recentMatches.size).toInt()}%"
            val rScore = recentMatches.sumOf { it.score?.toLongOrNull() ?: 0L }
            binding.tvRecentAvgDmg.text = "近期均分: ${rScore / recentMatches.size}"
        }
    }

    private fun generateMapStats() {
        val mapStatList = allMatches.groupBy { it.mapId }.mapNotNull { (mapId, matches) ->
            if (mapId == null) return@mapNotNull null
            val mapInfo = ApiService.MAP_CONFIG[mapId]
            val subModeStats = matches.groupBy { it.subMode ?: "0" }.mapValues { it.value.size }
            MapStatData(mapId, mapInfo?.first ?: "未知", mapInfo?.third ?: "", matches.size, matches.count { it.isWin == "1" }, subModeStats)
        }.sortedByDescending { it.totalGames }
        binding.recyclerMapStats.adapter = MapStatAdapter(mapStatList)
    }

    private fun switchNav(showStats: Boolean) {
        binding.viewStats.visibility = if (showStats) View.VISIBLE else View.GONE
        binding.layoutCollection.visibility = if (showStats) View.GONE else View.VISIBLE
    }

    private fun loadCollectionData(type: Int) {
        currentType = type; updateTabStyle(type); binding.viewCollection.adapter = null
        if (type == 3) binding.viewCollection.layoutManager = LinearLayoutManager(this)
        else binding.viewCollection.layoutManager = GridLayoutManager(this, 3)

        lifecycleScope.launch {
            val list = when (type) {
                0 -> api.getWeaponCollection()
                1 -> api.getTrapCollection()
                2 -> api.getPluginCollection()
                3 -> api.getFragmentHome()
                else -> null
            }
            if (list != null) {
                if (type == 3) {
                    binding.viewCollection.adapter = FragmentAdapter(list.filter { it.itemProgress != null })
                } else {
                    val filtered = if (currentQualityFilter == 0) list else list.filter { it.quality == currentQualityFilter }
                    binding.viewCollection.adapter = CollectionAdapter(filtered)
                }
            } else Toast.makeText(this@MainActivity, "暂无数据", Toast.LENGTH_SHORT).show()
        }
    }

    private fun updateTabStyle(type: Int) {
        val active = ContextCompat.getColor(this, R.color.accent)
        val inactive = ContextCompat.getColor(this, R.color.text_sub)
        binding.tabWeapon.setTextColor(if (type == 0) active else inactive)
        binding.tabTrap.setTextColor(if (type == 1) active else inactive)
        binding.tabPlugin.setTextColor(if (type == 2) active else inactive)
        binding.tabFragment.setTextColor(if (type == 3) active else inactive)
    }
}