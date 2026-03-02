package com.example.nzmhelper

import android.os.Bundle
import android.view.View
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.nzmhelper.databinding.ActivityMatchDetailBinding
import kotlinx.coroutines.launch

class MatchDetailActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMatchDetailBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMatchDetailBinding.inflate(layoutInflater)
        setContentView(binding.root)

        val roomId = intent.getStringExtra("ROOM_ID") ?: ""
        val win = intent.getBooleanExtra("IS_WIN", false)
        val time = intent.getStringExtra("TIME") ?: ""

        binding.root.findViewById<View>(R.id.btnBack).setOnClickListener { finish() }
        binding.root.findViewById<TextView>(R.id.tvTitleMap).text = intent.getStringExtra("MAP_NAME") ?: "详情"

        binding.tvDetailResult.text = if (win) "胜利" else "失败"
        binding.tvDetailResult.setTextColor(ContextCompat.getColor(this, if (win) R.color.win_red else R.color.text_sub))

        binding.tvDetailTime.text = time

        binding.recyclerPlayers.layoutManager = LinearLayoutManager(this)

        if (roomId.isNotEmpty()) {
            lifecycleScope.launch {
                val detail = LoginActivity.apiService.getMatchDetail(roomId)
                if (detail != null) {
                    val list = ArrayList<Pair<PlayerDetail, Boolean>>()
                    val selfName = detail.self?.nickname

                    detail.self?.let { list.add(Pair(it, true)) }

                    detail.teammates?.filter { it.nickname != selfName }?.forEach {
                        list.add(Pair(it, false))
                    }

                    binding.recyclerPlayers.adapter = MatchPlayerAdapter(list)
                }
            }
        }
    }
}