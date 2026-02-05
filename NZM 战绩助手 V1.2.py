import customtkinter as ctk
import tkinter as tk
from tkinter import messagebox
import requests
import threading
import time
import json
import io
import os
import hashlib
from urllib.parse import unquote
from concurrent.futures import ThreadPoolExecutor
from PIL import Image, ImageTk, ImageDraw, ImageFont
import webbrowser 

# --- 全局配置 ---
CURRENT_VERSION = "V1.2"
UPDATE_URL = "https://wwaoi.lanzouu.com/b019vntx7a"
UPDATE_PWD_RAW = "bx9d" 
BLOG_URL = "http://mobaiya.icu/"

THEME = {
    "bg": "#0f172a", 
    "card_bg": "#1e293b", 
    "self_bg": "#0B1120",
    "accent": "#3b82f6", 
    "red": "#ef4444", 
    "green": "#10b981",
    "yellow": "#f59e0b",
    "text_main": "#ffffff", 
    "text_sub": "#94a3b8", 
    "border": "#334155",
    "quality_legendary": "#d4a84b",
    "quality_epic": "#a855f7",
    "quality_rare": "#3b82f6",
    "quality_common": "#10b981"
}

SEASON_ID = 1

NOTICE_TEXT = [
    {"icon": "⚠️", "title": "数据说明", "text": "官方数据为历史总计（不含机甲）。详细分析基于最近 30 天 / 100 场游戏记录。", "color": THEME["text_main"]},
    {"icon": "🕒", "title": "过期机制", "text": "本地保存的Cookie将在 24小时 后自动过期，需重新登录。", "color": THEME["accent"]},
    {"icon": "🚫", "title": "严禁盗卖", "text": "倒卖死全家 倒卖死全家 倒卖死全家 （重要的事情说三遍）", "color": THEME["red"], "bold": True},
]

AUTHOR_INFO = {
    "author": "MoBai", 
    "qq": "113333914",
    "blog": BLOG_URL,
    "github": "https://github.com/MoBai-Curt",
    "origin": "HaMan412", 
    "origin_url": "https://github.com/HaMan412"
}

# 地图配置
MAP_CONFIG = {
    "1000": {"name": "风暴峡谷", "mode": "机甲战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1000.png"},
    "1001": {"name": "风暴峡谷", "mode": "机甲战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1001.png"},
    "1002": {"name": "凯旋之地", "mode": "机甲战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1002.png"},
    "112": {"name": "黑暗复活节", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-112.png"},
    "114": {"name": "大都会", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-114.png"},
    "115": {"name": "冰点源起", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-115.png"},
    "12": {"name": "黑暗复活节", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-12.png"},
    "132": {"name": "飓风要塞", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-132.png"},
    "135": {"name": "苍穹之上", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-135.png"},
    "14": {"name": "大都会", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-14.png"},
    "16": {"name": "昆仑神宫", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-16.png"},
    "17": {"name": "精绝古城", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-17.png"},
    "21": {"name": "冰点源起", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-21.png"},
    "30": {"name": "猎场-新手关", "mode": "僵尸猎场", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-30.png"},
    "300": {"name": "空间站", "mode": "塔防战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-300.png"},
    "304": {"name": "20号星港", "mode": "塔防战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-304.png"},
    "306": {"name": "联盟大厦", "mode": "塔防战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-306.png"},
    "308": {"name": "塔防-新手关", "mode": "塔防战", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-308.png"},
    "321": {"name": "根除变异", "mode": "时空追猎", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-321.png"},
    "322": {"name": "夺回资料", "mode": "时空追猎", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-322.png"},
    "323": {"name": "猎杀南十字", "mode": "时空追猎", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-323.png"},
    "324": {"name": "追猎-新手关", "mode": "时空追猎", "icon": "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-324.png"}
}

DIFFICULTY_INFO = {
    "0": "默认", "1": "引导", "2": "普通", "3": "困难", 
    "4": "英雄", "5": "炼狱", "6": "折磨I", "7": "折磨II",
    "8": "折磨III", "9": "折磨IV", "10": "折磨V", "11": "折磨VI", 
    "32": "练习", "64": "最大值"
}

def get_font_path(bold=False):
    system = os.name
    if system == 'nt': return "C:/Windows/Fonts/msyhbd.ttc" if bold else "C:/Windows/Fonts/msyh.ttc"
    return "arial.ttf"

def format_num(n):
    try: return "{:,}".format(int(n))
    except: return "0"

def get_mode_name(map_id_str):
    if map_id_str in MAP_CONFIG:
        return MAP_CONFIG[map_id_str]["mode"]
    mid = int(map_id_str)
    if mid >= 1000: return "机甲战"
    if 300 <= mid < 400: return "塔防战"
    if 10 <= mid < 200: return "僵尸猎场"
    return "未知模式"

def get_quality_color(q):
    if q == 4: return THEME["quality_legendary"]
    if q == 3: return THEME["quality_epic"]
    if q == 2: return THEME["quality_rare"]
    return THEME["quality_common"]

# --- 图片加载与缓存 ---
class ImageProcessor:
    def __init__(self):
        self.executor = ThreadPoolExecutor(max_workers=32) 
        self.cache = {}
        if not os.path.exists("cache"):
            try: os.makedirs("cache")
            except: pass
        try:
            self.f_title = ImageFont.truetype(get_font_path(True), 24)
            self.f_sub = ImageFont.truetype(get_font_path(), 16)
            self.f_list = ImageFont.truetype(get_font_path(), 14)
        except:
            self.f_title = self.f_sub = self.f_list = ImageFont.load_default()

    def safe_config(self, widget, img):
        try:
            if widget and widget.winfo_exists(): widget.configure(image=img)
        except: pass

    def get_cache_path(self, url):
        try: return os.path.join("cache", hashlib.md5(url.encode('utf-8')).hexdigest() + ".png")
        except: return None

    def load_icon(self, url, size, widget, circle=False, radius=0, bg_color=None):
        if not url: return
        url = unquote(url)
        key = f"{url}_{size}_{circle}_{radius}_{bg_color}"
        if key in self.cache:
            self.safe_config(widget, self.cache[key])
            return
        self.executor.submit(self._dl_icon, url, size, widget, circle, radius, bg_color, key)

    def _dl_icon(self, url, size, widget, circle, radius, bg_color, key):
        try:
            img = None
            cache_path = self.get_cache_path(url)
            if cache_path and os.path.exists(cache_path):
                try: img = Image.open(cache_path).convert("RGBA")
                except: pass
            
            if img is None:
                headers = {
                    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36',
                    'Referer': 'https://qzone.qq.com/'
                }
                res = requests.get(url, headers=headers, timeout=5)
                if res.status_code == 200:
                    img = Image.open(io.BytesIO(res.content)).convert("RGBA")
                    if cache_path:
                        try: img.save(cache_path, "PNG")
                        except: pass
            
            if img:
                if bg_color:
                    bg = Image.new('RGBA', img.size, bg_color)
                    bg.paste(img, (0, 0), img)
                    img = bg

                img = img.resize(size, Image.Resampling.LANCZOS)
                
                if circle:
                    mask = Image.new('L', size, 0)
                    ImageDraw.Draw(mask).ellipse((0, 0) + size, fill=255)
                    img.putalpha(mask)
                elif radius > 0:
                    mask = Image.new('L', size, 0)
                    draw = ImageDraw.Draw(mask)
                    draw.rounded_rectangle((0, 0) + size, radius=radius, fill=255)
                    img.putalpha(mask)

                ctk_img = ctk.CTkImage(img, size=size)
                self.cache[key] = ctk_img
                widget.after(0, lambda: self.safe_config(widget, ctk_img))
        except: pass

    def load_map_card(self, data, size, widget):
        url = data.get('icon')
        if not url: return
        key = f"map_{data['name']}_{size}"
        if key in self.cache:
            self.safe_config(widget, self.cache[key])
            return
        self.executor.submit(self._gen_map_card, url, data, size, widget, key)

    def _gen_map_card(self, url, data, size, widget, key):
        try:
            cache_path = self.get_cache_path(url)
            base_img = None
            if cache_path and os.path.exists(cache_path):
                try: base_img = Image.open(cache_path).convert("RGBA")
                except: pass
            if base_img is None:
                res = requests.get(url, timeout=5)
                base_img = Image.open(io.BytesIO(res.content)).convert("RGBA")
                if cache_path:
                    try: base_img.save(cache_path, "PNG")
                    except: pass

            w, h = size
            base_img = base_img.resize((w, h), Image.Resampling.LANCZOS)
            
            overlay = Image.new('RGBA', base_img.size, (0,0,0,0))
            draw = ImageDraw.Draw(overlay)
            for y in range(int(h * 0.3), h):
                alpha = int(255 * ((y - h * 0.3) / (h * 0.7)) * 0.95)
                draw.line([(0, y), (w, y)], fill=(8, 12, 20, alpha))
            
            out = Image.alpha_composite(base_img, overlay)
            draw = ImageDraw.Draw(out)
            
            padding = 15
            total = data.get('total', 0)
            wins = data.get('wins', 0)
            draw.text((padding, 15), data['name'], font=self.f_title, fill="#ffffff")
            rate = int(wins/total*100) if total > 0 else 0
            draw.text((padding, 50), f"{total}场 - {rate}% 胜率", font=self.f_sub, fill="#e2e8f0")

            if 'diffs' in data:
                diff_items = sorted(data['diffs'].items(), key=lambda x: x[1].get('total', 0), reverse=True)
                y_pos = h - 25
                for diff_name, d_val in diff_items[:4]:
                    d_total = d_val.get('total', 0)
                    d_rate = int(d_val.get('wins', 0)/d_total*100) if d_total > 0 else 0
                    draw.text((padding, y_pos), diff_name, font=self.f_list, fill="#94a3b8")
                    txt_w = draw.textlength(f"{d_total}场 ({d_rate}%)", font=self.f_list)
                    draw.text((w - padding - txt_w, y_pos), f"{d_total}场 ({d_rate}%)", font=self.f_list, fill="#ffffff")
                    y_pos -= 20

            ctk_img = ctk.CTkImage(out, size=size)
            self.cache[key] = ctk_img
            widget.after(0, lambda: self.safe_config(widget, ctk_img))
        except: pass

# --- API ---
class NZMApi:
    def __init__(self):
        self.session = requests.Session()
        self.cookie = ""
        self.auth_mode = "qq" 
        self.api_url = 'https://comm.ams.game.qq.com/ide/'
        self.headers_common = {
            "Host": "comm.ams.game.qq.com",
            "Connection": "keep-alive",
            "Content-Type": "application/x-www-form-urlencoded;",
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 MicroMessenger/7.0.20.1781(0x6700143B) NetType/WIFI MiniProgramEnv/Windows WindowsWechat/WMPF WindowsWechat(0x63090a13) UnifiedPCWindowsWechat(0xf254171e) XWEB/18787",
            "Referer": "https://servicewechat.com/wx4e8cbe4fb0eca54c/9/page-frame.html",
            "Accept-Encoding": "gzip, deflate, br",
            "Accept-Language": "zh-CN,zh;q=0.9"
        }

    def load_local(self):
        if os.path.exists("nzm_session.json"):
            try:
                with open("nzm_session.json") as f:
                    d = json.load(f)
                    if time.time() - d.get("t", 0) < 86400: 
                        self.cookie = d.get("c", "")
                        self.auth_mode = d.get("type", "qq")
                        return True
            except: pass
        return False

    def save_local(self):
        with open("nzm_session.json", "w") as f: 
            json.dump({"c": self.cookie, "t": time.time(), "type": self.auth_mode}, f)

    def set_cookie(self, cookie, mode="wx"):
        self.auth_mode = mode
        self.cookie = cookie
        self.save_local()
        return True

    def get_qr(self):
        try:
            url = f'https://ssl.ptlogin2.qq.com/ptqrshow?appid=549000912&e=2&l=M&s=3&d=72&v=4&t=0.5{int(time.time()*1000)}&daid=5&pt_3rd_aid=0'
            r = self.session.get(url)
            self.qrsig = r.cookies.get('qrsig')
            return r.content
        except: return None

    def check_qr(self):
        if not hasattr(self, 'qrsig'): return -1
        e = 0
        for c in self.qrsig: e += (e<<5)+ord(c)
        token = 2147483647 & e
        url = f'https://ssl.ptlogin2.qq.com/ptqrlogin?u1=https%3A%2F%2Fqzone.qq.com%2F&ptqrtoken={token}&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=0-0-{int(time.time()*1000)}&js_ver=21020514&js_type=1&login_sig=&pt_uistyle=40&aid=549000912&daid=5&'
        try:
            r = self.session.get(url, cookies={'qrsig': self.qrsig})
            if "ptuiCB('0'" in r.text:
                self.cookie = f"uin={self.session.cookies.get('uin')}; skey={self.session.cookies.get('skey')};"
                self.auth_mode = "qq"
                self.save_local()
                return 0
            return 65 if "ptuiCB('65'" in r.text else 66
        except: return -1

    def _post(self, m, p):
        if not self.cookie: return None
        try:
            d = {'iChartId': '430662', 'iSubChartId': '430662', 'sIdeToken': 'NoOapI', 'method': m, 'from_source': '2', 'param': json.dumps(p)}
            h = self.headers_common.copy()
            h['Cookie'] = self.cookie
            res = self.session.post(self.api_url, headers=h, data=d)
            if res.status_code == 200:
                json_data = res.json()
                if json_data.get('ret') == 0:
                    return json_data.get('jData',{}).get('data',{}).get('data',{})
            return None
        except: return None

    def get_user_summary(self):
        return self._post('center.user.stats', {"seasonID": SEASON_ID})

    def get_home_collection(self):
        res = self._post('collection.home', {"seasonID": SEASON_ID, "limit": 6})
        return res.get('weaponList', []) if res else []

    def get_all_history(self):
        all_data = []
        for p in range(1, 11):
            res = self._post('center.user.game.list', {"seasonID": SEASON_ID, "page": p, "limit": 10})
            if not res: break
            lst = res.get('gameList', [])
            if not lst: break
            all_data.extend(lst)
            time.sleep(0.05)
        return all_data

    def get_collection(self, t): 
        k = {'weapon':'collection.weapon.list','trap':'collection.trap.list','plugin':'collection.plugin.list'}.get(t)
        p = {"seasonID": SEASON_ID, "queryTime": True} if t=='weapon' else {"seasonID": SEASON_ID}
        return self._post(k, p) or {}

# --- 公告 ---
class NoticeWindow(ctk.CTkToplevel):
    def __init__(self, master):
        super().__init__(master)
        self.title("使用说明 & 公告")
        self.geometry("600x650")
        self.configure(fg_color=THEME["bg"])
        self.transient(master) 
        
        header = ctk.CTkFrame(self, fg_color="transparent", height=60)
        header.pack(fill="x", padx=20, pady=20)
        ctk.CTkLabel(header, text="关于 & 说明", font=("微软雅黑", 24, "bold"), text_color="white").pack(side="left")

        scroll = ctk.CTkScrollableFrame(self, fg_color="transparent")
        scroll.pack(fill="both", expand=True, padx=10, pady=(0, 20))

        for item in NOTICE_TEXT:
            self.create_section(scroll, item)
        self.create_author_card(scroll)
        
    def create_section(self, parent, item):
        frame = ctk.CTkFrame(parent, fg_color=THEME["card_bg"], corner_radius=10)
        frame.pack(fill="x", pady=5, padx=10)
        title_frame = ctk.CTkFrame(frame, fg_color="transparent")
        title_frame.pack(fill="x", padx=15, pady=(15, 5))
        ctk.CTkLabel(title_frame, text=item["icon"], font=("Segoe UI Emoji", 18)).pack(side="left", padx=(0, 10))
        ctk.CTkLabel(title_frame, text=item["title"], font=("微软雅黑", 15, "bold"), text_color=item.get("color", "#fff")).pack(side="left")
        msg = ctk.CTkLabel(frame, text=item["text"], font=("微软雅黑", 13), text_color=item.get("color", "#cbd5e1"), wraplength=500, justify="left", anchor="w")
        msg.pack(fill="x", padx=45, pady=(0, 15))

    def create_author_card(self, parent):
        card = ctk.CTkFrame(parent, fg_color=THEME["self_bg"], border_width=1, border_color=THEME["border"], corner_radius=10)
        card.pack(fill="x", pady=20, padx=10)
        
        ctk.CTkLabel(card, text="作者信息", font=("微软雅黑", 16, "bold"), text_color=THEME["accent"]).pack(pady=(15, 5))
        
        info_frame = ctk.CTkFrame(card, fg_color="transparent")
        info_frame.pack(pady=10)
        
        ctk.CTkLabel(info_frame, text=f"作者: {AUTHOR_INFO['author']}", font=("微软雅黑", 13), text_color="gray").pack()
        ctk.CTkLabel(info_frame, text=f"QQ: {AUTHOR_INFO['qq']}", font=("微软雅黑", 13), text_color="gray").pack()
        ctk.CTkLabel(info_frame, text=f"原作者: {AUTHOR_INFO['origin']}", font=("微软雅黑", 13), text_color="gray").pack()

        btn_frame = ctk.CTkFrame(card, fg_color="transparent")
        btn_frame.pack(pady=(15, 20))
        
        ctk.CTkButton(btn_frame, text="开源地址 (GitHub)", fg_color="transparent", border_width=1, border_color=THEME["accent"], 
                      text_color=THEME["accent"], width=140, height=35, 
                      command=lambda: webbrowser.open(AUTHOR_INFO['github'])).pack(side="left", padx=10)
                      
        ctk.CTkButton(btn_frame, text="原作者 GitHub", fg_color="transparent", border_width=1, border_color=THEME["accent"], 
                      text_color=THEME["accent"], width=140, height=35, 
                      command=lambda: webbrowser.open(AUTHOR_INFO['origin_url'])).pack(side="left", padx=10)

# --- 战绩详情页 (修复配装显示) ---
class MatchDetailWindow(ctk.CTkToplevel):
    def __init__(self, master, rid, api, proc):
        super().__init__(master)
        self.title("对局详情")
        self.geometry("1440x900")
        self.configure(fg_color=THEME["bg"])
        self.api = api; self.proc = proc
        
        self.scroll = ctk.CTkScrollableFrame(self, fg_color="transparent")
        self.scroll.pack(fill="both", expand=True, padx=20, pady=20)
        
        ctk.CTkLabel(self.scroll, text="加载中...", text_color="gray").pack(pady=50)
        threading.Thread(target=self.load, args=(rid,), daemon=True).start()

    def load(self, rid):
        data = self.api._post('center.game.detail', {"seasonID": SEASON_ID, "roomID": rid})
        self.after(0, lambda: self.render(data) if self.winfo_exists() else None)

    def render(self, data):
        if not self.winfo_exists(): return
        for w in self.scroll.winfo_children(): w.destroy()
        if not data: return ctk.CTkLabel(self.scroll, text="加载失败", text_color="red").pack()
        
        self_info = data.get('loginUserDetail', {})
        others = [p for p in data.get('list',[]) if p.get('nickname') != self_info.get('nickname')]
        try: others.sort(key=lambda x: int(x.get('baseDetail',{}).get('iScore',0)), reverse=True)
        except: pass

        grid = ctk.CTkFrame(self.scroll, fg_color="transparent")
        grid.pack(fill="x", pady=(0, 20))
        grid.grid_columnconfigure((0,1,2), weight=1)

        for i, p in enumerate(others):
            self.create_teammate_card(grid, p, i)
        
        self.create_self_bar(self_info)
        
        self.equip_container = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.equip_container.pack(fill="x", pady=5)

    def create_teammate_card(self, parent, data, idx):
        row, col = divmod(idx, 3)
        card = ctk.CTkFrame(parent, fg_color=THEME["card_bg"], corner_radius=8)
        card.grid(row=row, column=col, padx=5, pady=5, sticky="ew")
        
        header = ctk.CTkFrame(card, fg_color="transparent")
        header.pack(fill="x", padx=10, pady=10)
        
        ava = ctk.CTkLabel(header, text="", width=55, height=55, fg_color="transparent", corner_radius=25)
        ava.pack(side="left")
        if data.get('avatar'): self.proc.load_icon(data['avatar'], (55, 55), ava, circle=True)

        info_box = ctk.CTkFrame(header, fg_color="transparent")
        info_box.pack(side="left", padx=10)
        
        nick = unquote(data.get('nickname','Unknown'))
        base = data.get('baseDetail', {})
        hunt = data.get('huntingDetails', {})
        
        ctk.CTkLabel(info_box, text=nick, font=("微软雅黑", 13, "bold"), text_color="white").pack(anchor="w")
        ctk.CTkLabel(info_box, text=f"积分: {format_num(base.get('iScore'))}", font=("微软雅黑", 11), text_color="#ccc").pack(anchor="w")
        ctk.CTkLabel(info_box, text=f"BOSS: {format_num(hunt.get('DamageTotalOnBoss'))}", font=("微软雅黑", 10), text_color="#888").pack(anchor="w")

        if data.get('equipmentScheme'):
            btn = ctk.CTkButton(card, text="查看配装 ▼", height=24, fg_color="#334155", 
                                command=lambda: self.toggle_equip(card, data['equipmentScheme']))
            btn.pack(fill="x", padx=10, pady=(0, 10))

    def create_self_bar(self, data):
        bar = ctk.CTkFrame(self.scroll, fg_color=THEME["self_bg"], border_width=1, border_color="#333", height=130, corner_radius=10)
        bar.pack(fill="x", pady=10)
        
        left = ctk.CTkFrame(bar, fg_color="transparent", width=140, height=130)
        left.place(x=0, y=0)
        
        ava = ctk.CTkLabel(left, text="", width=64, height=64, fg_color="transparent", corner_radius=32)
        ava.place(relx=0.5, y=25, anchor="n")
        if data.get('avatar'): self.proc.load_icon(data['avatar'], (64, 64), ava, circle=True)
        
        ctk.CTkLabel(left, text=unquote(data.get('nickname','')), font=("微软雅黑", 12, "bold"), text_color="white").place(relx=0.5, y=95, anchor="n")

        base = data.get('baseDetail', {})
        hunt = data.get('huntingDetails', {})
        stats = [
            ("积分", base.get('iScore')), ("击杀", base.get('iKills')), ("死亡", base.get('iDeaths')),
            ("BOSS伤害", hunt.get('DamageTotalOnBoss')), ("小怪伤害", hunt.get('DamageTotalOnMobs')), ("金币", hunt.get('totalCoin'))
        ]
        start_x = 160; gap = 130
        for i, (k, v) in enumerate(stats):
            x = start_x + i * gap
            ctk.CTkLabel(bar, text=k, font=("微软雅黑", 10), text_color="#888").place(x=x, y=35, anchor="nw")
            ctk.CTkLabel(bar, text=format_num(v), font=("Impact", 20), text_color="white").place(x=x, y=60, anchor="nw")

        if data.get('equipmentScheme'):
            btn = ctk.CTkButton(bar, text="查看我的配装", width=120, height=28, fg_color="#334155", 
                                command=lambda: self.toggle_equip_self(data['equipmentScheme']))
            btn.place(relx=0.98, rely=0.1, anchor="ne")

    def toggle_equip_self(self, scheme):
        for w in self.equip_container.winfo_children(): w.destroy()
        
        if getattr(self, 'is_self_expanded', False):
            self.is_self_expanded = False
            return

        self.is_self_expanded = True
        
        panel = ctk.CTkFrame(self.equip_container, fg_color="#0f172a", corner_radius=6)
        panel.pack(fill="x", padx=5, pady=5)
        
        ctk.CTkLabel(panel, text="我的配装", font=("微软雅黑", 12, "bold"), text_color=THEME["accent"]).pack(anchor="w", padx=10, pady=5)
        
        grid = ctk.CTkFrame(panel, fg_color="transparent")
        grid.pack(fill="x", padx=10, pady=10)
        
        for i, item in enumerate(scheme):
            self.render_weapon_item(grid, i, item)

    def toggle_equip(self, parent, scheme):
        existing = None
        for child in parent.winfo_children():
            if isinstance(child, ctk.CTkFrame) and getattr(child, "is_equip_panel", False):
                existing = child
                break
        
        if existing:
            existing.destroy()
            return

        panel = ctk.CTkFrame(parent, fg_color="#0f172a", corner_radius=6)
        panel.is_equip_panel = True
        panel.pack(fill="x", padx=10, pady=10)

        for i, item in enumerate(scheme):
            self.render_weapon_item(panel, i, item)

    def render_weapon_item(self, parent, col, item):
        q = item.get('quality', 1)
        color = get_quality_color(q)
        
        card = ctk.CTkFrame(parent, fg_color="#1e293b", border_width=1, border_color=color, width=110)
        card.grid(row=0, column=col, padx=5, pady=5)
        
        img_lbl = ctk.CTkLabel(card, text="", width=94, height=48)
        img_lbl.pack(pady=5)
        if item.get('pic'): self.proc.load_icon(item['pic'], (94, 48), img_lbl)
        
        ctk.CTkLabel(card, text=item.get('weaponName',''), font=("微软雅黑", 10), text_color=color).pack()

        plugins = item.get('commonItems', [])
        if plugins:
            pg = ctk.CTkFrame(card, fg_color="transparent")
            pg.pack(pady=5)
            for j, p in enumerate(plugins):
                r, c = divmod(j, 2)
                p_lbl = ctk.CTkLabel(pg, text="", width=34, height=34, fg_color="#334155", corner_radius=4)
                p_lbl.grid(row=r, column=c, padx=2, pady=2)
                if p.get('pic'): self.proc.load_icon(p['pic'], (34, 34), p_lbl)

# --- 登录界面 (含公告) ---
class LoginFrame(ctk.CTkFrame):
    def __init__(self, master, api, proc, cb):
        super().__init__(master, fg_color=THEME["bg"])
        self.api = api; self.proc = proc; self.cb = cb; self.polling = False
        
        self.create_login_notice(self)

        self.tab = ctk.CTkTabview(self, fg_color=THEME["card_bg"], width=400, height=450)
        self.tab.pack(pady=20)
        
        self.t_qq = self.tab.add("QQ登录 (Q区)")
        self.t_wx = self.tab.add("微信登录 (微区)")
        
        ctk.CTkLabel(self.t_qq, text="方式一：手机QQ扫码", font=("微软雅黑", 14, "bold")).pack(pady=(10, 5))
        self.qr = ctk.CTkLabel(self.t_qq, text="", width=120, height=120, fg_color="black")
        self.qr.pack(pady=5)
        self.stat = ctk.CTkLabel(self.t_qq, text="点击获取二维码", text_color="gray", font=("微软雅黑", 12))
        self.stat.pack(pady=5)
        self.btn = ctk.CTkButton(self.t_qq, text="获取二维码", width=120, height=30, command=self.start_qq)
        self.btn.pack(pady=10)
        
        ctk.CTkFrame(self.t_qq, height=2, fg_color="#334155").pack(fill="x", pady=10, padx=20)
        ctk.CTkLabel(self.t_qq, text="方式二：手动抓包Cookie", font=("微软雅黑", 14, "bold"), text_color="#f59e0b").pack(pady=5)
        self.qq_cookie_entry = ctk.CTkTextbox(self.t_qq, height=80, width=320)
        self.qq_cookie_entry.pack(pady=5)
        self.btn_qq_manual = ctk.CTkButton(self.t_qq, text="Cookie登录", width=120, height=30, command=self.manual_qq)
        self.btn_qq_manual.pack(pady=10)

        ctk.CTkLabel(self.t_wx, text="粘贴抓包获取的 Cookie", font=("微软雅黑", 16, "bold")).pack(pady=(20, 10))
        self.cookie_entry = ctk.CTkTextbox(self.t_wx, height=150, width=300)
        self.cookie_entry.pack(pady=10)
        ctk.CTkLabel(self.t_wx, text="提示: Cookie需包含 ieg_ams_token", text_color="gray", font=("微软雅黑", 12)).pack(pady=5)
        self.btn_wx = ctk.CTkButton(self.t_wx, text="登录查询", command=self.manual_wx)
        self.btn_wx.pack(pady=20)

    def create_login_notice(self, parent):
        frame = ctk.CTkFrame(parent, fg_color=THEME["card_bg"], corner_radius=10, width=400)
        frame.pack(pady=(40, 10), padx=20)
        
        head = ctk.CTkFrame(frame, fg_color="transparent")
        head.pack(fill="x", padx=15, pady=10)
        ctk.CTkLabel(head, text="公告 & 关于", font=("微软雅黑", 16, "bold"), text_color="white").pack(side="left")
        
        btn_box = ctk.CTkFrame(head, fg_color="transparent")
        btn_box.pack(side="right")
        ctk.CTkButton(btn_box, text="检查更新", width=80, height=24, fg_color=THEME["green"], command=self.check_update).pack(side="left", padx=5)
        ctk.CTkButton(btn_box, text="作者信息", width=80, height=24, fg_color=THEME["accent"], command=self.show_author_info).pack(side="left", padx=5)

        for item in NOTICE_TEXT:
            row = ctk.CTkFrame(frame, fg_color="transparent")
            row.pack(fill="x", padx=15, pady=2)
            ctk.CTkLabel(row, text=item["icon"], width=25).pack(side="left")
            ctk.CTkLabel(row, text=item["title"], font=("微软雅黑", 12, "bold"), text_color=item["color"], width=70, anchor="w").pack(side="left")
            ctk.CTkLabel(row, text=item["text"], font=("微软雅黑", 12), text_color="#cbd5e1", anchor="w").pack(side="left")

    def show_author_info(self):
        NoticeWindow(self) 
        
    def check_update(self):
        if messagebox.askyesno("检查更新", f"当前版本: {CURRENT_VERSION}\n发现新版本！\n\n点击【是】将自动复制密码 [{UPDATE_PWD_RAW}] 并跳转下载。"):
            self.clipboard_clear()
            self.clipboard_append(UPDATE_PWD_RAW)
            self.update()
            webbrowser.open(UPDATE_URL)

    def start_qq(self):
        self.btn.configure(state="disabled")
        threading.Thread(target=self.fetch_qr, daemon=True).start()

    def fetch_qr(self):
        raw = self.api.get_qr()
        if raw:
            img = ctk.CTkImage(Image.open(io.BytesIO(raw)), size=(120,120))
            self.after(0, lambda: [self.qr.configure(image=img), self.stat.configure(text="请扫码")])
            self.polling = True
            threading.Thread(target=self.poll, daemon=True).start()
        else: self.after(0, lambda: self.btn.configure(state="normal"))

    def poll(self):
        while self.polling:
            ret = self.api.check_qr()
            if ret == 0: 
                self.polling=False
                self.after(0, self.cb)
                break
            elif ret == 65: 
                self.polling=False
                self.after(0, lambda: [self.stat.configure(text="二维码过期"), self.btn.configure(state="normal")])
                break
            time.sleep(2)

    def manual_qq(self):
        self._manual_login(self.qq_cookie_entry.get("1.0", "end"), "qq")

    def manual_wx(self):
        self._manual_login(self.cookie_entry.get("1.0", "end"), "wx")

    def _manual_login(self, raw, mode):
        raw_cookie = raw.strip()
        if not raw_cookie:
            messagebox.showerror("错误", "请输入 Cookie")
            return
        
        if raw_cookie.lower().startswith("cookie:"):
            raw_cookie = raw_cookie[7:].strip()
            
        if "ieg_ams_token" not in raw_cookie:
            if not messagebox.askyesno("警告", "Cookie似乎不完整(缺少ieg_ams_token)，可能导致查询失败，是否继续?"):
                return
                
        if self.api.set_cookie(raw_cookie, mode):
            threading.Thread(target=self.verify_login, daemon=True).start()
            
    def verify_login(self):
        res = self.api.get_all_history()
        if res is not None:
             self.after(0, self.cb)
        else:
             self.after(0, lambda: messagebox.showerror("登录失败", "Cookie 无效或已过期，请重新抓包获取。"))

    def reset(self): 
        self.polling=False
        self.qr.configure(image=None)
        self.btn.configure(state="normal")

# --- 主看板 ---
class MainFrame(ctk.CTkFrame):
    def __init__(self, master, api, proc, logout_cb):
        super().__init__(master, fg_color=THEME["bg"])
        self.api = api; self.proc = proc
        
        self.wep_data = []; self.plug_data = []; self.trap_data = []

        nav = ctk.CTkFrame(self, height=60, fg_color=THEME["bg"])
        nav.pack(fill="x")
        
        zone_name = "QQ区" if self.api.auth_mode == "qq" else "微信区"
        ctk.CTkLabel(nav, text=f"NZM 战绩 ({zone_name})", font=("微软雅黑", 20, "bold"), text_color="white").pack(side="left", padx=30, pady=15)
        
        ctk.CTkButton(nav, text="注销", width=60, fg_color=THEME["red"], command=logout_cb).pack(side="right", padx=20)
        ctk.CTkButton(nav, text="刷新", width=60, command=self.init_data).pack(side="right", padx=5)

        self.tabs = ctk.CTkTabview(self, fg_color="transparent", segmented_button_fg_color=THEME["card_bg"],
                                   segmented_button_selected_color=THEME["accent"], segmented_button_unselected_color=THEME["card_bg"], text_color="#fff")
        self.tabs.pack(fill="both", expand=True, padx=20, pady=10)
        self.t1 = self.tabs.add("战绩")
        self.t2 = self.tabs.add("图鉴")

        self.scroll = ctk.CTkScrollableFrame(self.t1, fg_color="transparent")
        self.scroll.pack(fill="both", expand=True)
        
        self.create_notice_board(self.scroll)

        self.create_head(self.scroll, "官方历史数据 (不含机甲/PVP)")
        self.summary_frame = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.summary_frame.pack(fill="x", pady=5)

        self.create_head(self.scroll, "近期战绩统计 (最近30天/100场)")
        self.recent_frame = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.recent_frame.pack(fill="x", pady=5)

        self.create_head(self.scroll, "模式统计")
        self.mode_frame = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.mode_frame.pack(fill="x", pady=5)

        self.create_head(self.scroll, "碎片进度")
        self.fragment_frame = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.fragment_frame.pack(fill="x", pady=5)

        self.create_head(self.scroll, "地图详情 (近100场)")
        self.map_grid = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.map_grid.pack(fill="x", pady=10)
        
        self.create_head(self.scroll, "对局记录")
        self.hist_grid = ctk.CTkFrame(self.scroll, fg_color="transparent")
        self.hist_grid.pack(fill="x", pady=10)

        self.wep_head = ctk.CTkFrame(self.t2, fg_color="transparent")
        self.wep_head.pack(fill="x", pady=10)
        self.wep_grid = ctk.CTkScrollableFrame(self.t2, fg_color="transparent")
        self.wep_grid.pack(fill="both", expand=True)
        
        self.render_coll_ui("weapon") 

    def create_notice_board(self, parent):
        frame = ctk.CTkFrame(parent, fg_color=THEME["card_bg"], corner_radius=10)
        frame.pack(fill="x", pady=(10, 5), padx=5)
        
        head = ctk.CTkFrame(frame, fg_color="transparent")
        head.pack(fill="x", padx=15, pady=10)
        ctk.CTkLabel(head, text="公告 & 关于", font=("微软雅黑", 16, "bold"), text_color="white").pack(side="left")
        
        btn_box = ctk.CTkFrame(head, fg_color="transparent")
        btn_box.pack(side="right")
        
        ctk.CTkButton(btn_box, text="检查更新", width=80, height=24, fg_color=THEME["green"], command=self.check_update).pack(side="left", padx=5)
        ctk.CTkButton(btn_box, text="作者信息", width=80, height=24, fg_color=THEME["accent"], command=self.show_author_info).pack(side="left", padx=5)

        for item in NOTICE_TEXT:
            row = ctk.CTkFrame(frame, fg_color="transparent")
            row.pack(fill="x", padx=15, pady=2)
            ctk.CTkLabel(row, text=item["icon"], width=25).pack(side="left")
            ctk.CTkLabel(row, text=item["title"], font=("微软雅黑", 12, "bold"), text_color=item["color"], width=70, anchor="w").pack(side="left")
            ctk.CTkLabel(row, text=item["text"], font=("微软雅黑", 12), text_color="#cbd5e1", anchor="w").pack(side="left")

    def show_author_info(self):
        NoticeWindow(self)

    def check_update(self):
        if messagebox.askyesno("检查更新", f"当前版本: {CURRENT_VERSION}\n发现新版本！\n\n点击【是】将自动复制密码 [{UPDATE_PWD_RAW}] 并跳转下载。"):
            self.clipboard_clear()
            self.clipboard_append(UPDATE_PWD_RAW)
            self.update()
            webbrowser.open(UPDATE_URL)

    def create_head(self, p, t):
        f = ctk.CTkFrame(p, fg_color="transparent")
        f.pack(fill="x", pady=(10, 5))
        ctk.CTkFrame(f, width=4, height=18, fg_color=THEME["accent"]).pack(side="left")
        ctk.CTkLabel(f, text=t, font=("微软雅黑", 14, "bold"), text_color="#ffffff").pack(side="left", padx=10)

    def init_data(self):
        threading.Thread(target=self._load, daemon=True).start()

    def _load(self):
        with ThreadPoolExecutor(max_workers=6) as executor:
            f_summary = executor.submit(self.api.get_user_summary)
            f_frags = executor.submit(self.api.get_home_collection)
            f_games = executor.submit(self.api.get_all_history)
            f_wep = executor.submit(self.api.get_collection, 'weapon')
            f_plug = executor.submit(self.api.get_collection, 'plugin')
            f_trap = executor.submit(self.api.get_collection, 'trap')
            
            self.summary_data = f_summary.result()
            self.frag_data = f_frags.result()
            games = f_games.result()
            self.wep_data = f_wep.result().get('list', [])
            self.plug_data = f_plug.result().get('list', [])
            self.trap_data = f_trap.result().get('list', [])
        
        self.after(0, lambda: self.render_summary(self.summary_data))
        self.after(0, lambda: self.render_fragments(self.frag_data))
        if games: self.after(0, lambda: self.render_stats(games))
        self.after(0, lambda: self.render_coll_ui("weapon"))

    def render_summary(self, data):
        for w in self.summary_frame.winfo_children(): w.destroy()
        if not data: return
        grid = ctk.CTkFrame(self.summary_frame, fg_color="transparent")
        grid.pack(fill="x")
        grid.grid_columnconfigure((0, 1), weight=1)

        card1 = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=100)
        card1.grid(row=0, column=0, padx=5, pady=5, sticky="ew")
        ctk.CTkLabel(card1, text="历史总场次", font=("微软雅黑", 12), text_color="#94a3b8").place(x=15, y=15)
        ctk.CTkLabel(card1, text=str(data.get('huntGameCount', 0)), font=("微软雅黑", 28, "bold"), text_color="white").place(x=15, y=45)

        playtime = int(data.get('playtime', 0) or 0)
        hours = playtime // 60
        card2 = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=100)
        card2.grid(row=0, column=1, padx=5, pady=5, sticky="ew")
        ctk.CTkLabel(card2, text="历史总时长", font=("微软雅黑", 12), text_color="#94a3b8").place(x=15, y=15)
        ctk.CTkLabel(card2, text=f"{hours}时", font=("微软雅黑", 28, "bold"), text_color="white").place(x=15, y=45)

    def render_fragments(self, fragments):
        for w in self.fragment_frame.winfo_children(): w.destroy()
        if not fragments: 
            ctk.CTkLabel(self.fragment_frame, text="暂无碎片进度", text_color="gray").pack()
            return

        grid = ctk.CTkFrame(self.fragment_frame, fg_color="transparent")
        grid.pack(fill="x")
        grid.grid_columnconfigure((0, 1), weight=1)

        for i, frag in enumerate(fragments):
            r, c = divmod(i, 2)
            card = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=80)
            card.grid(row=r, column=c, padx=5, pady=5, sticky="ew")
            
            icon_frame = ctk.CTkFrame(card, width=100, height=50, fg_color="#0f172a", corner_radius=6)
            icon_frame.place(x=10, y=15)
            icon_lbl = ctk.CTkLabel(icon_frame, text="", width=100, height=50)
            icon_lbl.place(x=0, y=0)
            
            if frag.get('pic'): 
                self.proc.load_icon(frag['pic'], (100, 50), icon_lbl, radius=6)

            ctk.CTkLabel(card, text=frag.get('weaponName','未知'), font=("微软雅黑", 14, "bold"), text_color="white").place(x=120, y=10)
            
            prog = frag.get('itemProgress', {})
            curr = prog.get('current', 0)
            req = prog.get('required', 100)
            val = curr / req if req > 0 else 0
            
            pbar = ctk.CTkProgressBar(card, height=8, width=150, progress_color=THEME["yellow"])
            pbar.place(x=120, y=45)
            pbar.set(val)
            ctk.CTkLabel(card, text=f"{curr}/{req}", font=("微软雅黑", 12), text_color="#94a3b8").place(x=120, y=55)

    def render_stats(self, games):
        self._render_recent_stats(games)
        self._render_mode_stats(games)
        
        for w in self.map_grid.winfo_children(): w.destroy()
        self.map_grid.grid_columnconfigure((0,1,2), weight=1)
        
        stats = {}
        for g in games:
            mid = str(g.get('iMapId'))
            if int(mid) >= 1000 or g.get('iGameMode') == 6: continue
            name = MAP_CONFIG.get(mid, {}).get('name', f'地图{mid}')
            icon = MAP_CONFIG.get(mid, {}).get('icon', '')
            if name not in stats: stats[name] = {'total':0, 'wins':0, 'diffs':{}, 'icon': icon}
            stats[name]['total'] += 1
            if str(g.get('iIsWin')) == '1': stats[name]['wins'] += 1
            diff = DIFFICULTY_INFO.get(str(g.get('iSubModeType')), "普通")
            if diff not in stats[name]['diffs']: stats[name]['diffs'][diff] = {'total':0, 'wins':0}
            stats[name]['diffs'][diff]['total'] += 1
            if str(g.get('iIsWin')) == '1': stats[name]['diffs'][diff]['wins'] += 1

        for i, (name, data) in enumerate(stats.items()):
            r, c = divmod(i, 3)
            card = ctk.CTkFrame(self.map_grid, fg_color="transparent") 
            card.grid(row=r, column=c, padx=10, pady=10, sticky="ew")
            lbl = ctk.CTkLabel(card, text="")
            lbl.pack(fill="x")
            data['name'] = name
            self.proc.load_map_card(data, (380, 200), lbl)

        for w in self.hist_grid.winfo_children(): w.destroy()
        for g in games[:20]:
            self.create_hist_row(g)

    def _render_recent_stats(self, games):
        for w in self.recent_frame.winfo_children(): w.destroy()
        grid = ctk.CTkFrame(self.recent_frame, fg_color="transparent")
        grid.pack(fill="x")
        grid.grid_columnconfigure((0,1,2), weight=1)

        total_games = len(games)
        wins = len([g for g in games if str(g.get('iIsWin')) == '1'])
        win_rate = (wins / total_games * 100) if total_games > 0 else 0
        total_dmg = sum([int(g.get('iScore', 0)) for g in games])
        avg_dmg = total_dmg // total_games if total_games > 0 else 0

        c1 = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=100)
        c1.grid(row=0, column=0, padx=5, pady=5, sticky="ew")
        ctk.CTkLabel(c1, text="近期场次", font=("微软雅黑", 12), text_color="#94a3b8").place(x=15, y=15)
        ctk.CTkLabel(c1, text=str(total_games), font=("微软雅黑", 28, "bold"), text_color="white").place(x=15, y=45)

        c2 = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=100)
        c2.grid(row=0, column=1, padx=5, pady=5, sticky="ew")
        ctk.CTkLabel(c2, text="近期胜率", font=("微软雅黑", 12), text_color="#94a3b8").place(x=15, y=15)
        ctk.CTkLabel(c2, text=f"{win_rate:.1f}%", font=("微软雅黑", 28, "bold"), text_color="white").place(x=15, y=45)

        c3 = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=100)
        c3.grid(row=0, column=2, padx=5, pady=5, sticky="ew")
        ctk.CTkLabel(c3, text="场均伤害", font=("微软雅黑", 12), text_color="#94a3b8").place(x=15, y=15)
        ctk.CTkLabel(c3, text=format_num(avg_dmg), font=("微软雅黑", 28, "bold"), text_color="white").place(x=15, y=45)

    def _render_mode_stats(self, games):
        for w in self.mode_frame.winfo_children(): w.destroy()
        grid = ctk.CTkFrame(self.mode_frame, fg_color="transparent")
        grid.pack(fill="x")
        grid.grid_columnconfigure((0,1,2), weight=1)

        modes = {"僵尸猎场": {"total":0, "win":0}, "时空追猎": {"total":0, "win":0}, "塔防战": {"total":0, "win":0}}
        for g in games:
            mode = get_mode_name(g.get('iMapId'))
            if mode in modes:
                modes[mode]["total"] += 1
                if str(g.get('iIsWin')) == '1': modes[mode]["win"] += 1
        
        for i, (mname, mdata) in enumerate(modes.items()):
            card = ctk.CTkFrame(grid, fg_color=THEME["card_bg"], height=100)
            card.grid(row=0, column=i, padx=5, pady=5, sticky="ew")
            
            ctk.CTkLabel(card, text=mname, font=("微软雅黑", 12), text_color="#94a3b8").place(x=15, y=15)
            ctk.CTkLabel(card, text=f"{mdata['total']} 场", font=("微软雅黑", 24, "bold"), text_color="white").place(x=15, y=40)
            
            rate = (mdata['win'] / mdata['total'] * 100) if mdata['total'] > 0 else 0
            detail_text = f"胜 {mdata['win']} 负 {mdata['total']-mdata['win']} {rate:.1f}%"
            ctk.CTkLabel(card, text=detail_text, font=("微软雅黑", 11), text_color="#94a3b8").place(x=15, y=75)

    # --- 修复：时长和分数右对齐 ---
    def create_hist_row(self, g):
        rid = g.get('DsRoomId')
        row = ctk.CTkFrame(self.hist_grid, fg_color=THEME["card_bg"], height=85, corner_radius=6)
        row.pack(fill="x", pady=4)
        
        mid = str(g.get('iMapId'))
        icon_url = MAP_CONFIG.get(mid, {}).get('icon')
        img_lbl = ctk.CTkLabel(row, text="", width=140, height=75)
        img_lbl.place(x=5, y=5)
        if icon_url: self.proc.load_icon(icon_url, (140, 75), img_lbl, radius=4)

        is_win = str(g.get('iIsWin')) == '1'
        result_text = "胜利" if is_win else "失败"
        result_color = THEME["red"] if is_win else "#94a3b8"
        mode_name = get_mode_name(mid)
        
        ctk.CTkLabel(row, text=result_text, font=("微软雅黑", 18, "bold"), text_color=result_color).place(x=160, y=15)
        ctk.CTkLabel(row, text=mode_name, font=("微软雅黑", 14), text_color=THEME["red"]).place(x=210, y=18)

        map_name = MAP_CONFIG.get(mid, {}).get('name', mid)
        diff_name = DIFFICULTY_INFO.get(str(g.get('iSubModeType')), "普通")
        dt = g.get('dtGameStartTime', '')[5:-3] 
        info_text = f"{map_name}-{diff_name}  {dt}"
        ctk.CTkLabel(row, text=info_text, font=("微软雅黑", 12), text_color="#64748b").place(x=160, y=50)

        score = format_num(g.get('iScore'))
        dur_sec = int(g.get('iDuration', 0))
        dur_str = f"{dur_sec//60}分{dur_sec%60}秒"

        right_box = ctk.CTkFrame(row, fg_color="transparent")
        right_box.pack(side="right", padx=20, pady=10)
        
        score_lbl = ctk.CTkLabel(right_box, text=score, font=("Impact", 28), text_color=THEME["red"], anchor="e")
        score_lbl.pack(anchor="e")
        
        dur_lbl = ctk.CTkLabel(right_box, text=dur_str, font=("微软雅黑", 14, "bold"), text_color="white", anchor="e")
        dur_lbl.pack(anchor="e", pady=(5, 0))
        
        for w in [row, img_lbl] + row.winfo_children():
            w.bind("<Button-1>", lambda e: MatchDetailWindow(self, rid, self.api, self.proc))

    def render_coll_ui(self, c_type):
        for w in self.wep_head.winfo_children(): w.destroy()
        left = ctk.CTkFrame(self.wep_head, fg_color="transparent")
        left.pack(side="left")
        ctk.CTkFrame(left, width=4, height=24, fg_color=THEME["accent"]).pack(side="left") 
        
        title_map = {"weapon": "武器图鉴", "plugin": "插件图鉴", "trap": "塔防图鉴"}
        items = self.wep_data if c_type == "weapon" else (self.plug_data if c_type == "plugin" else self.trap_data)
        owned = len([x for x in items if x.get('owned')])
        ctk.CTkLabel(left, text=f"{title_map[c_type]} ({owned}/{len(items)})", font=("微软雅黑", 18, "bold"), text_color="#fff").pack(side="left", padx=10)
        
        right = ctk.CTkFrame(self.wep_head, fg_color="transparent")
        right.pack(side="right")
        
        if c_type == "weapon":
            self.mk_btn(right, "全部", lambda: self.render_grid(items, 'q', 'all'), True)
            self.mk_btn(right, "传说", lambda: self.render_grid(items, 'q', 4))
            self.mk_btn(right, "史诗", lambda: self.render_grid(items, 'q', 3))
        elif c_type == "plugin":
            self.mk_btn(right, "全部", lambda: self.render_grid(items, 's', 'all'), True)
            self.mk_btn(right, "一号", lambda: self.render_grid(items, 's', 1))
            self.mk_btn(right, "二号", lambda: self.render_grid(items, 's', 2))
            self.mk_btn(right, "三号", lambda: self.render_grid(items, 's', 3))
            self.mk_btn(right, "四号", lambda: self.render_grid(items, 's', 4))
        self.render_grid(items, 'q' if c_type=='weapon' else 's', 'all')
        
        if not hasattr(self, 'switch_rendered'):
            s = ctk.CTkFrame(self.t2, fg_color="transparent")
            s.pack(side="top", fill="x", pady=5)
            for k, v in title_map.items():
                ctk.CTkButton(s, text=v[:2], width=60, fg_color="#333", command=lambda k=k: self.render_coll_ui(k)).pack(side="left", padx=2)
            self.switch_rendered = True

    def mk_btn(self, p, t, cmd, act=False):
        c = THEME["accent"] if act else "transparent"
        ctk.CTkButton(p, text=t, width=50, height=24, fg_color=c, border_width=1, border_color="#444", command=cmd).pack(side="left", padx=2)

    def render_grid(self, items, k, v):
        for w in self.wep_grid.winfo_children(): w.destroy()
        self.wep_grid.grid_columnconfigure((0,1,2,3,4,5), weight=1)
        flt = items if v=='all' else [x for x in items if str(x.get('quality' if k=='q' else 'slotIndex')) == str(v)]
        flt.sort(key=lambda x: (not x.get('owned'), -(x.get('quality',0))))
        for i, item in enumerate(flt): 
            r, c = divmod(i, 6)
            self.mk_card(self.wep_grid, r, c, item)

    def mk_card(self, p, r, c, item):
        owned = item.get('owned', False)
        q = item.get('quality', 1)
        bg = THEME["card_bg"] if owned else "#1a1a1a"
        bd = {4:"#f59e0b", 3:"#a855f7", 2:"#3b82f6"}.get(q, "#333")
        is_weapon = 'weaponName' in item
        
        card = ctk.CTkFrame(p, fg_color=bg, border_width=1, border_color=bd if owned else "#222")
        card.grid(row=r, column=c, padx=5, pady=5, sticky="nsew")
        
        if is_weapon:
            lbl = ctk.CTkLabel(card, text="", fg_color="transparent")
            lbl.pack(fill="x", expand=True, padx=5, pady=(10, 2))
            img_size = (120, 65)
        else:
            lbl = ctk.CTkLabel(card, text="", width=60, height=60, fg_color="transparent")
            lbl.pack(pady=8)
            img_size = (60, 60)
        
        url = item.get('pic') or item.get('icon')
        if url: self.proc.load_icon(url, img_size, lbl)
        name = item.get('weaponName') or item.get('itemName') or item.get('trapName')
        ctk.CTkLabel(card, text=name, font=("微软雅黑", 10), text_color="white" if owned else "gray").pack(pady=(0, 5))

class App(ctk.CTk):
    def __init__(self):
        super().__init__()
        self.api = NZMApi()
        self.proc = ImageProcessor()
        self.title(f"NZM 战绩助手 {CURRENT_VERSION} (双区版)")
        self.geometry("1440x900")
        self.configure(fg_color=THEME["bg"])
        
        self.main_frame = None
        self.login_frame = LoginFrame(self, self.api, self.proc, self.show_main)
        
        if self.api.load_local(): self.show_main()
        else: self.login_frame.pack(fill="both", expand=True)

    def show_main(self):
        self.login_frame.pack_forget()
        self.main_frame = MainFrame(self, self.api, self.proc, self.logout)
        self.main_frame.pack(fill="both", expand=True)
        self.main_frame.init_data()

    def logout(self):
        if os.path.exists("nzm_session.json"): os.remove("nzm_session.json")
        if self.main_frame: self.main_frame.destroy()
        self.login_frame.pack(fill="both", expand=True)
        self.login_frame.reset()

if __name__ == "__main__":
    app = App()
    app.mainloop()
