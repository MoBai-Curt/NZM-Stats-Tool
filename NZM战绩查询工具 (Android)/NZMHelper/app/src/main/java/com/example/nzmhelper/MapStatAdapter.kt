package com.example.nzmhelper

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.nzmhelper.databinding.ItemMapStatBinding

data class MapStatData(
    val mapId: String,
    val mapName: String,
    val mapPic: String,
    val totalGames: Int,
    val winGames: Int,
    val subModeStats: Map<String, Int>
)

class MapStatAdapter(private val items: List<MapStatData>) : RecyclerView.Adapter<MapStatAdapter.ViewHolder>() {

    class ViewHolder(val binding: ItemMapStatBinding) : RecyclerView.ViewHolder(binding.root)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ItemMapStatBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = items[position]
        val context = holder.itemView.context

        holder.binding.tvMapName.text = item.mapName

        if (item.mapPic.isNotEmpty()) {
            Glide.with(context).load(item.mapPic).into(holder.binding.imgMapIcon)
        }

        val winRate = if (item.totalGames > 0) (item.winGames * 100 / item.totalGames) else 0

        val statsBuilder = StringBuilder()
        statsBuilder.append("${item.totalGames}场 - ${winRate}% 胜率\n")

        val subModeList = item.subModeStats.entries.map {
            val diffName = ApiService.getSubModeName(it.key)
            val name = if (diffName.isNotEmpty()) diffName else "未知难度"
            "$name: ${it.value}场"
        }

        if (subModeList.isNotEmpty()) {
            statsBuilder.append(subModeList.joinToString(" | "))
        }

        holder.binding.tvMapTotal.text = statsBuilder.toString()
    }

    override fun getItemCount() = items.size
}