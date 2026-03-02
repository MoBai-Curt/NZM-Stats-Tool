package com.example.nzmhelper

import android.content.Intent
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.nzmhelper.databinding.ItemGameBinding

class GameAdapter(private val games: List<GameRecord>) : RecyclerView.Adapter<GameAdapter.ViewHolder>() {

    class ViewHolder(val binding: ItemGameBinding) : RecyclerView.ViewHolder(binding.root)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ItemGameBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val game = games[position]
        val context = holder.itemView.context
        val isWin = game.isWin == "1"

        holder.binding.tvResult.text = if (isWin) "胜利" else "失败"
        holder.binding.tvResult.setTextColor(ContextCompat.getColor(context, if (isWin) R.color.win_red else R.color.text_sub))
        holder.binding.tvScore.text = "得分: ${game.score ?: "0"}"

        val duration = game.duration?.toIntOrNull() ?: 0
        holder.binding.tvTime.text = "${duration / 60}分${duration % 60}秒"

        val mapInfo = ApiService.MAP_CONFIG[game.mapId]
        val startTime = if (!game.startTime.isNullOrEmpty()) game.startTime!!.substringAfter("-") else ""

        val diffName = ApiService.getSubModeName(game.subMode)
        val diffTag = if (diffName.isNotEmpty()) "[$diffName]" else ""

        holder.binding.tvMapName.text = "${mapInfo?.first ?: "未知地图"} $diffTag $startTime"

        if (mapInfo != null && !mapInfo.third.isNullOrEmpty()) {
            Glide.with(context).load(mapInfo.third).into(holder.binding.imgMap)
        }

        holder.itemView.setOnClickListener {
            if (!game.roomId.isNullOrEmpty()) {
                val intent = Intent(context, MatchDetailActivity::class.java).apply {
                    putExtra("ROOM_ID", game.roomId)
                    putExtra("MAP_NAME", mapInfo?.first ?: "详情")
                    putExtra("IS_WIN", isWin)
                    putExtra("TIME", game.startTime ?: "")
                }
                context.startActivity(intent)
            }
        }
    }

    override fun getItemCount() = games.size
}